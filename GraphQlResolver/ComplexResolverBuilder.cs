using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public class ComplexResolverBuilder : IComplexResolverBuilder
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod("Add");
        private readonly IGraphQlResolvable contract;
        private readonly Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> resolve;
        private readonly Type modelType;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        public ComplexResolverBuilder(IGraphQlResolvable contract, Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> resolve, Type modelType)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.modelType = modelType;
            this.expressions = ImmutableDictionary<string, IGraphQlResult>.Empty;
        }

        protected ComplexResolverBuilder(IGraphQlResolvable contract, Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> resolve, ImmutableDictionary<string, IGraphQlResult> expressions, Type modelType)
            : this(contract, resolve, modelType)
        {
            this.expressions = expressions;
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new ComplexResolverBuilder(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        public IGraphQlResult Build()
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

        public IComplexResolverBuilder Add(string property, IDictionary<string, object?>? parameters) =>
            Add(property, property, parameters);

        public IComplexResolverBuilder Add(string displayName, string property, IDictionary<string, object?>? parameters)
        {
            var result = contract.ResolveQuery(property, parameters: parameters ?? ImmutableDictionary<string, object?>.Empty);
            // TODO - prevent non-primitives from being final return after adding. If this result is a non-primitive, client will be getting raw domain value!
            //if (!IsGraphQlPrimitive(TypeSystem.GetElementType(result.ResultType) ?? result.ResultType))
            //{
            //    throw new InvalidOperationException("Cannot use simple resolution for complex type");
            //}
            return new ComplexResolverBuilder(contract, resolve, expressions
                .Add(displayName ?? property, result), modelType);
        }

        public bool IsType(string value) =>
            contract.IsType(value);

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            if (contract.IsType(value))
            {
                return (IComplexResolverBuilder)typedBuilder((IComplexResolverBuilder)this);
            }
            return this;
        }
    }
}
