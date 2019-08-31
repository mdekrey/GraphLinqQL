﻿using System;
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
        private readonly Func<LambdaExpression, IGraphQlResult<TFinal>> resolve;
        private readonly Type modelType;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        public ComplexResolverBuilder(TContract contract, Func<LambdaExpression, IGraphQlResult<TFinal>> resolve, Type modelType)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.modelType = modelType;
            this.expressions = ImmutableDictionary<string, IGraphQlResult>.Empty;
        }

        protected ComplexResolverBuilder(TContract contract, Func<LambdaExpression, IGraphQlResult<TFinal>> resolve, ImmutableDictionary<string, IGraphQlResult> expressions, Type modelType)
            : this(contract, resolve, modelType)
        {
            this.expressions = expressions;
        }

        public IComplexResolverBuilder<TContract, TFinal> Add(string property, params object[] parameters) => Add(property, property, parameters);

        public IComplexResolverBuilder<TContract, TFinal> Add(string displayName, string property, params object[] parameters)
        {
            var result = contract.ResolveQuery(property, parameters: parameters);
            return new ComplexResolverBuilder<TContract, TFinal>(contract, resolve, expressions
                .Add(displayName, result), modelType);
        }


        public IComplexResolverBuilder<TContract, TFinal> Add(string displayName, Func<TContract, IGraphQlResult<object>> resolve)
        {
            return new ComplexResolverBuilder<TContract, TFinal>(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)), modelType);
        }

        public IGraphQlResult<TFinal> Build()
        {
            var modelParameter = Expression.Parameter(modelType);


            var variable = Expression.Parameter(typeof(Dictionary<string, object>));

            // TODO - all joins first, create a lookup of values based on join expression, then use that in the next foreach

            var resultDictionary = Expression.ListInit(Expression.New(variable.Type), expressions.Select(result =>
            {
                var inputResolver = result.Value.UntypedResolver;
                var resolveBody = inputResolver.Body.Replace(inputResolver.Parameters[0], with: modelParameter);
                return Expression.ElementInit(addMethod, Expression.Constant(result.Key), Expression.Convert(resolveBody, typeof(object)));
            }));
            var func = Expression.Lambda(resultDictionary, modelParameter);

            return resolve(func);
        }
    }
}
