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
        public Action<FieldContext> FieldContextSetup { get; }
        public Dictionary<string, IGraphQlScalarResult> Results { get; } = new Dictionary<string, IGraphQlScalarResult>();

        public ComplexResolutionEntry(IGraphQlResolvable graphQlResolvable, Type domainType, Action<FieldContext> fieldContextSetup)
        {
            this.GraphQlResolvable = graphQlResolvable;
            DomainType = domainType;
            FieldContextSetup = fieldContextSetup;
        }
    }

    internal class ComplexResolverBuilder : IComplexResolverBuilder
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary<string, object>.Add))!;
        private readonly Func<IReadOnlyList<LambdaExpression>, IGraphQlScalarResult<object>> resolve;
        private readonly Type modelType;
        private readonly IReadOnlyList<ComplexResolutionEntry> resolvers;

        protected ComplexResolverBuilder(
            IReadOnlyList<ComplexResolutionEntry> resolvers,
            Func<IReadOnlyList<LambdaExpression>, IGraphQlScalarResult<object>> resolve,
            Type modelType)
        {
            this.resolvers = resolvers;
            this.resolve = resolve;
            this.modelType = modelType;
        }

        internal ComplexResolverBuilder(
            IContract contractMappings,
            IGraphQlServiceProvider serviceProvider,
            Func<IReadOnlyList<LambdaExpression>, IGraphQlScalarResult<object>> resolve,
            Type modelType)
            : this(contractMappings.Resolvables.Select(e => CreateComplexResolution(serviceProvider, e)).ToArray(), resolve, modelType)
        {
        }

        private static ComplexResolutionEntry CreateComplexResolution(IGraphQlServiceProvider serviceProvider, ContractEntry entry)
        {
            var modelType = entry.Contract.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>)).First().GetGenericArguments()[0];
            var contract = CreateContract(entry.Contract, serviceProvider);
            var accepts = contract as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            return new ComplexResolutionEntry(
                contract, 
                entry.DomainType, 
                (fieldContext) =>
                {
                    contract.FieldContext = fieldContext;
                    accepts.Original = GraphQlResultFactory.Construct(fieldContext, modelType);
                }
            );
        }

        private static IGraphQlResolvable CreateContract(Type contractType, IGraphQlServiceProvider serviceProvider)
        {
            var contract = serviceProvider.GetResolverContract(contractType);
            
            return contract;
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, IReadOnlyList<QueryLocation> locations, Func<IGraphQlResolvable, IGraphQlScalarResult<object>> resolve)
        {
            foreach (var r in resolvers)
            {
                var fieldContext = CreateFieldContext(r.GraphQlResolvable.GraphQlTypeName, displayName, locations);
                try
                {
                    r.FieldContextSetup(fieldContext);
                    r.Results.Add(displayName, resolve(r.GraphQlResolvable));
                }
                catch (Exception ex)
                {
                    if (ex.HasGraphQlErrors(out var errors))
                    {
                        foreach (var error in errors)
                        {
                            error.Fixup(fieldContext);
                        }
                    }
                    else
                    {
                        ex.AddGraphQlError(WellKnownErrorCodes.UnhandledError, fieldContext);
                    }
                    throw;
                }
            }
            return this;
        }

        private static FieldContext CreateFieldContext(string graphQlTypeName, string displayName, IReadOnlyList<QueryLocation> locations)
        {
            return new FieldContext(graphQlTypeName, displayName, locations);
        }

        public IGraphQlScalarResult<object> Build()
        {
            var mainSelectors = resolvers.Select(r =>
            {
                var allJoins = r.Results.Values.SelectMany(v => v.Joins).ToImmutableHashSet();
                var expressions = r.Results.ToDictionary(result => result.Key, result => result.Value.ConstructResult());
                var inputParam = Expression.Parameter(r.DomainType);
                var resultDictionary = Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), expressions.Select(result =>
                {
                    var inputResolver = result.Value.Inline(inputParam);
                    return Expression.ElementInit(addMethod, Expression.Constant(result.Key), inputResolver.Box());
                }));
                var resultLambda = Expression.Lambda(resultDictionary, inputParam);
                return Expression.Lambda(resultLambda.Body.Replace(allJoins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Inline(resultLambda.Parameters[0]))), resultLambda.Parameters);
            }).ToArray();
            return resolve(mainSelectors);
        }

        public IComplexResolverBuilder Add(string property, IReadOnlyList<QueryLocation> locations, IGraphQlParameterResolver? parameters) =>
            Add(property, property, locations, parameters);

        public IComplexResolverBuilder Add(string displayName, string property, IReadOnlyList<QueryLocation> locations, IGraphQlParameterResolver? parameters)
        {
            foreach (var r in resolvers)
            {
                var fieldContext = CreateFieldContext(r.GraphQlResolvable.GraphQlTypeName, property, locations);
                r.FieldContextSetup(fieldContext);
                var result = SafeResolve(r.GraphQlResolvable, property, parameters);

                r.Results.Add(displayName, result is IGraphQlScalarResult scalarResult
                        ? scalarResult
                        : throw new InvalidOperationException("Cannot use simple resolution for complex type").AddGraphQlError(WellKnownErrorCodes.RequiredSubselection, fieldContext));
            }
            return this;
        }

        private IGraphQlResult SafeResolve(IGraphQlResolvable contract, string property, IGraphQlParameterResolver? parameters)
        {
            try
            {
                return contract.ResolveQuery(property, parameters: parameters ?? BasicParameterResolver.Empty);
            }
            catch (Exception ex)
            {
                ex.AddGraphQlError(WellKnownErrorCodes.ErrorInResolver, contract.FieldContext);
                throw;
            }
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            if (typedBuilder == null)
            {
                throw new ArgumentNullException(nameof(typedBuilder));
            }

            typedBuilder(new ComplexResolverBuilder(this.resolvers.Where(r => r.GraphQlResolvable.IsType(value)).ToArray(), resolve, modelType));

            return this;
        }
    }
}
