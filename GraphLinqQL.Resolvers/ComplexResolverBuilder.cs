using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    class ComplexResolutionEntry
    {
        public IGraphQlResolvable GraphQlResolvable { get; }
        public Type DomainType { get; }
        public Dictionary<string, IGraphQlScalarResult> Results { get; } = new Dictionary<string, IGraphQlScalarResult>();

        public ComplexResolutionEntry(IGraphQlResolvable graphQlResolvable, Type domainType)
        {
            this.GraphQlResolvable = graphQlResolvable;
            DomainType = domainType;
        }
    }

    internal class ComplexResolverBuilder : IComplexResolverBuilder
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary<string, object>.Add))!;
        private readonly Func<IReadOnlyList<LambdaExpression>, IGraphQlScalarResult> resolve;
        private readonly Type modelType;
        private readonly FieldContext fieldContext;
        private readonly IReadOnlyList<ComplexResolutionEntry> resolvers;

        protected ComplexResolverBuilder(
            IReadOnlyList<ComplexResolutionEntry> resolvers,
            Func<IReadOnlyList<LambdaExpression>, IGraphQlScalarResult> resolve,
            Type modelType,
            FieldContext fieldContext)
        {
            this.resolvers = resolvers;
            this.resolve = resolve;
            this.modelType = modelType;
            this.fieldContext = fieldContext;
        }

        internal ComplexResolverBuilder(
            IContract contractMappings,
            IGraphQlServiceProvider serviceProvider,
            Func<IReadOnlyList<LambdaExpression>, IGraphQlScalarResult> resolve,
            Type modelType,
            FieldContext fieldContext)
            : this(contractMappings.Resolvables.Select(e => new ComplexResolutionEntry(CreateContract(e.Contract, serviceProvider), e.DomainType)).ToArray(), resolve, modelType, fieldContext)
        {
        }

        private static IGraphQlResolvable CreateContract(Type contractType, IGraphQlServiceProvider serviceProvider)
        {
            var contract = serviceProvider.GetResolverContract(contractType);
            var accepts = contract as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            var modelType = accepts.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>)).First().GetGenericArguments()[0];
            accepts.Original = GraphQlResultFactory.Construct(modelType);
            return contract;
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, FieldContext context, Func<IGraphQlResolvable, IGraphQlScalarResult> resolve)
        {
            foreach (var r in resolvers)
            {
                r.Results.Add(displayName, resolve(r.GraphQlResolvable));
            }
            return this;
        }

        public IGraphQlScalarResult Build()
        {
            var resultDictionary = resolvers.Select(r =>
            {
                var expressions = r.Results.ToDictionary(result => result.Key, result => result.Value.ConstructResult());
                var inputParam = Expression.Parameter(r.DomainType);
                var resultDictionary = Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), expressions.Select(result =>
                {
                    var inputResolver = result.Value.Inline(inputParam);
                    return Expression.ElementInit(addMethod, Expression.Constant(result.Key), inputResolver.Box());
                }));
                return Expression.Lambda(resultDictionary, inputParam);
            }).ToArray();
            // TODO - should log an error instead of just adding a null to the array - what if it is a not-null array?

            if (resolvers.SelectMany(r => r.Results.Values).SelectMany(v => v.Joins).Any() && resultDictionary.Length > 1)
            {
                throw new NotImplementedException("GraphLinqQL does not currently support joins within unions");
            }

            var allJoins = resolvers.SelectMany(r => r.Results.Values).SelectMany(v => v.Joins).ToImmutableHashSet();

            var mainSelectors = resultDictionary
                .Select(r => Expression.Lambda(r.Body.Replace(allJoins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Inline(r.Parameters[0]))), r.Parameters))
                .ToArray();
            return resolve(mainSelectors);
        }

        public IComplexResolverBuilder Add(string property, FieldContext context, IGraphQlParameterResolver? parameters) =>
            Add(property, property, context, parameters);

        public IComplexResolverBuilder Add(string displayName, string property, FieldContext context, IGraphQlParameterResolver? parameters)
        {
            foreach (var r in resolvers)
            {
                var result = SafeResolve(r.GraphQlResolvable, property, context, parameters);

                r.Results.Add(displayName, result is IGraphQlScalarResult scalarResult
                        ? scalarResult
                        : throw new InvalidOperationException("Cannot use simple resolution for complex type").AddGraphQlError(WellKnownErrorCodes.RequiredSubselection, context.Locations, new { fieldName = property, type = r.GraphQlResolvable }));
            }
            return this;
        }

        private IGraphQlResult SafeResolve(IGraphQlResolvable contract, string property, FieldContext context, IGraphQlParameterResolver? parameters)
        {
            try
            {
                return contract.ResolveQuery(property, context, parameters: parameters ?? BasicParameterResolver.Empty);
            }
            catch (Exception ex)
            {
                ex.AddGraphQlError(WellKnownErrorCodes.ErrorInResolver, context.Locations, new { fieldName = property, type = contract.GraphQlTypeName });
                throw;
            }
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            if (typedBuilder == null)
            {
                throw new ArgumentNullException(nameof(typedBuilder));
            }

            typedBuilder(new ComplexResolverBuilder(this.resolvers.Where(r => r.GraphQlResolvable.IsType(value)).ToArray(), resolve, modelType, fieldContext));

            return this;
        }
    }
}
