﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class GraphQlFinalizerResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        private readonly IGraphQlResult original;
        private readonly Func<LambdaExpression, LambdaExpression> postProcess;

        public GraphQlFinalizerResult(IGraphQlResult original, Func<LambdaExpression, LambdaExpression> postProcess)
        {
            this.original = original;
            this.postProcess = postProcess;
        }

        public static GraphQlFinalizerResult<TReturnType>  Inline(IGraphQlResult original, LambdaExpression expreesionToInline)
        {
            return new GraphQlFinalizerResult<TReturnType>(original, original => Expression.Lambda(expreesionToInline.Inline(original.Body), original.Parameters));
        }

        public IGraphQlParameterResolverFactory ParameterResolverFactory => original.ParameterResolverFactory;

        public LambdaExpression UntypedResolver => postProcess(original.UntypedResolver);

        public Type? Contract => original.Contract;

        public IReadOnlyCollection<IGraphQlJoin> Joins => original.Joins;

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> => 
            new GraphQlFinalizerResult<TContract>(original.AsContract(typeof(TContract)), postProcess);

        public IGraphQlResult AsContract(Type contract) =>
            (IGraphQlResult)Activator.CreateInstance(typeof(GraphQlFinalizerResult<>).MakeGenericType(contract), new object[] { original.AsContract(contract), postProcess });

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider) =>
            new PostResolveComplexResolverBuilder(original.ResolveComplex(serviceProvider), newResult => new GraphQlFinalizerResult<TReturnType>(newResult, postProcess));
    }
}