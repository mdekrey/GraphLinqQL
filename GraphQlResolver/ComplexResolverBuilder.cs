using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public class ComplexResolverBuilder<TContract> : IComplexResolverBuilder<TContract>
        where TContract : IGraphQlResolvable
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod("Add");
        private readonly TContract contract;
        private readonly Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> resolve;
        private readonly Type modelType;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        public ComplexResolverBuilder(TContract contract, Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> resolve, Type modelType)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.modelType = modelType;
            this.expressions = ImmutableDictionary<string, IGraphQlResult>.Empty;
        }

        protected ComplexResolverBuilder(TContract contract, Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> resolve, ImmutableDictionary<string, IGraphQlResult> expressions, Type modelType)
            : this(contract, resolve, modelType)
        {
            this.expressions = expressions;
        }

        public IComplexResolverBuilder<TContract> Add(string property, IDictionary<string, object?>? parameters = null) => Add(property, property, parameters);

        public IComplexResolverBuilder<TContract> Add(string displayName, string property, IDictionary<string, object?>? parameters = null)
        {
            var result = contract.ResolveQuery(property, parameters: parameters ?? ImmutableDictionary<string, object?>.Empty);
            // TODO - prevent non-primitives from being final return after adding. If this result is a non-primitive, client will be getting raw domain value!
            //if (!IsGraphQlPrimitive(TypeSystem.GetElementType(result.ResultType) ?? result.ResultType))
            //{
            //    throw new InvalidOperationException("Cannot use simple resolution for complex type");
            //}
            return new ComplexResolverBuilder<TContract>(contract, resolve, expressions
                .Add(displayName ?? property, result), modelType);
        }


        public IComplexResolverBuilder<TContract> Add(string displayName, Func<TContract, IGraphQlResult> resolve)
        {
            return new ComplexResolverBuilder<TContract>(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new ComplexResolverBuilder<TContract>(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        private const bool PerformNullCheck = true;
        public IGraphQlResult Build()
        {
            var modelParameter = Expression.Parameter(modelType, "ComplexResolverBuilder " + modelType.FullName);

            var allJoins = expressions.SelectMany(e => e.Value.Joins).ToImmutableHashSet();

            var resultDictionary = Expression.Convert(Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), expressions.Select(result =>
            {
                var inputResolver = result.Value.UntypedResolver;
                var resolveBody = inputResolver.Body.Replace(inputResolver.Parameters[0], with: modelParameter);
                return Expression.ElementInit(addMethod, Expression.Constant(result.Key), Expression.Convert(resolveBody, typeof(object)));
            })), typeof(IDictionary<string, object>));
            var returnResult = PerformNullCheck
                ? Expressions.SafeNull(modelParameter, resultDictionary)
                : resultDictionary;
            var func = Expression.Lambda(returnResult, modelParameter);

            return resolve(func, allJoins);
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string property, IDictionary<string, object?>? parameters) => 
            Add(property, parameters);

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, string property, IDictionary<string, object?>? parameters) => 
            Add(displayName, property, parameters);

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
