using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public class ComplexResolverBuilder : IComplexResolverBuilder
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary<string, object>.Add))!;
        private readonly IGraphQlResolvable contract;
        private readonly Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlScalarResult> resolve;
        private readonly Type modelType;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        protected ComplexResolverBuilder(
            IGraphQlResolvable contract,
            Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlScalarResult> resolve,
            ImmutableDictionary<string, IGraphQlResult> expressions,
            Type modelType)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.modelType = modelType;
            this.expressions = expressions;
        }

        public ComplexResolverBuilder(
            Type contractType,
            IGraphQlServiceProvider serviceProvider,
            Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlScalarResult> resolve,
            Type modelType)
            : this(CreateContract(contractType, serviceProvider, modelType), resolve, ImmutableDictionary<string, IGraphQlResult>.Empty, modelType)
        {
        }

        private static IGraphQlResolvable CreateContract(Type contractType, IGraphQlServiceProvider serviceProvider, Type modelType)
        {
            var contract = serviceProvider.GetResolverContract(contractType);
            var accepts = contract as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            accepts.Original = GraphQlResultFactory.Construct(modelType);
            return contract;
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, FieldContext context, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new ComplexResolverBuilder(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        public IGraphQlScalarResult Build()
        {
            var modelParameter = Expression.Parameter(modelType, "ComplexResolverBuilder " + modelType.FullName);

            var allJoins = expressions.SelectMany(e => e.Value.Joins).ToImmutableHashSet();

            var resultDictionary = Expression.Convert(Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), expressions.Select(result =>
            {
                var inputResolver = result.Value.UntypedResolver;
                var resolveBody = inputResolver.Inline(modelParameter);
                return Expression.ElementInit(addMethod, Expression.Constant(result.Key), Expression.Convert(resolveBody, typeof(object)));
            })), typeof(object));
            var func = Expression.Lambda(resultDictionary, modelParameter);

            return resolve(func, allJoins);
        }

        public IComplexResolverBuilder Add(string property, FieldContext context, IGraphQlParameterResolver? parameters) =>
            Add(property, property, context, parameters);

        public IComplexResolverBuilder Add(string displayName, string property, FieldContext context, IGraphQlParameterResolver? parameters)
        {
            IGraphQlResult result = SafeResolve(property, context, parameters);
            if (result.ShouldSubselect)
            {
                throw new InvalidOperationException("Cannot use simple resolution for complex type").AddGraphQlError(WellKnownErrorCodes.RequiredSubselection, context.Locations, new { fieldName = property, type = contract.GraphQlTypeName });
            }
            return new ComplexResolverBuilder(contract, resolve, expressions
                .Add(displayName ?? property, result), modelType);
        }

        private IGraphQlResult SafeResolve(string property, FieldContext context, IGraphQlParameterResolver? parameters)
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

        public bool IsType(string value) =>
            contract.IsType(value);

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            if (typedBuilder == null)
            {
                throw new ArgumentNullException(nameof(typedBuilder));
            }

            if (contract.IsType(value))
            {
                return (IComplexResolverBuilder)typedBuilder((IComplexResolverBuilder)this);
            }
            return this;
        }
    }
}
