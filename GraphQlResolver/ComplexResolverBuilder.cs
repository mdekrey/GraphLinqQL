using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public class ComplexResolverBuilder<TContract, TFinal> : IComplexResolverBuilder<TContract, TFinal>
        where TContract : IGraphQlResolvable
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod("Add");
        private readonly TContract contract;
        private readonly Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult<TFinal>> resolve;
        private readonly Type modelType;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        public ComplexResolverBuilder(TContract contract, Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult<TFinal>> resolve, Type modelType)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.modelType = modelType;
            this.expressions = ImmutableDictionary<string, IGraphQlResult>.Empty;
        }

        protected ComplexResolverBuilder(TContract contract, Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult<TFinal>> resolve, ImmutableDictionary<string, IGraphQlResult> expressions, Type modelType)
            : this(contract, resolve, modelType)
        {
            this.expressions = expressions;
        }

        public IComplexResolverBuilder<TContract, TFinal> Add(string property, IDictionary<string, object>? parameters = null) => Add(property, property, parameters);

        public IComplexResolverBuilder<TContract, TFinal> Add(string displayName, string property, IDictionary<string, object>? parameters = null)
        {
            var result = contract.ResolveQuery(property, parameters: parameters ?? ImmutableDictionary<string, object>.Empty);
            // TODO - prevent non-primitives
            //if (!IsGraphQlPrimitive(TypeSystem.GetElementType(result.ResultType) ?? result.ResultType))
            //{
            //    throw new InvalidOperationException("Cannot use simple resolution for complex type");
            //}
            return new ComplexResolverBuilder<TContract, TFinal>(contract, resolve, expressions
                .Add(displayName ?? property, result), modelType);
        }


        public IComplexResolverBuilder<TContract, TFinal> Add(string displayName, Func<TContract, IGraphQlResult<object>> resolve)
        {
            return new ComplexResolverBuilder<TContract, TFinal>(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        IComplexResolverBuilder<TFinal> IComplexResolverBuilder<TFinal>.Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new ComplexResolverBuilder<TContract, TFinal>(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        public IGraphQlResult<TFinal> Build()
        {
            var modelParameter = Expression.Parameter(modelType, "ComplexResolverBuilder " + modelType.FullName);

            var allJoins = expressions.SelectMany(e => e.Value.Joins).ToImmutableHashSet();

            var resultDictionary = Expression.Convert(Expression.ListInit(Expression.New(typeof(Dictionary<string, object>)), expressions.Select(result =>
            {
                var inputResolver = result.Value.UntypedResolver;
                var resolveBody = inputResolver.Body.Replace(inputResolver.Parameters[0], with: modelParameter);
                return Expression.ElementInit(addMethod, Expression.Constant(result.Key), Expression.Convert(resolveBody, typeof(object)));
            })), typeof(IDictionary<string, object>));
            var func = Expression.Lambda(resultDictionary, modelParameter);

            return resolve(func, allJoins);
        }

        IComplexResolverBuilder<TFinal> IComplexResolverBuilder<TFinal>.Add(string property, IDictionary<string, object>? parameters) => 
            Add(property, parameters);

        IComplexResolverBuilder<TFinal> IComplexResolverBuilder<TFinal>.Add(string displayName, string property, IDictionary<string, object>? parameters) => 
            Add(displayName, property, parameters);

        public bool IsType(string value) =>
            contract.IsType(value);

    }
}
