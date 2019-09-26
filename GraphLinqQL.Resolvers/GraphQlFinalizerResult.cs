using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class GraphQlFinalizerResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        private IGraphQlResult original;
        private LambdaExpression finalizer;

        public GraphQlFinalizerResult(IGraphQlResult original, LambdaExpression finalizer)
        {
            this.original = original;
            this.finalizer = finalizer;
        }

        public IGraphQlParameterResolverFactory ParameterResolverFactory => original.ParameterResolverFactory;

        public LambdaExpression UntypedResolver => Expression.Lambda(this.finalizer.Inline(original.UntypedResolver.Body), original.UntypedResolver.Parameters);

        public Type? Contract => original.Contract;

        public IReadOnlyCollection<IGraphQlJoin> Joins => original.Joins;

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> => 
            new GraphQlFinalizerResult<TContract>(original.AsContract(typeof(TContract)), finalizer);

        public IGraphQlResult AsContract(Type contract) =>
            (IGraphQlResult)Activator.CreateInstance(typeof(GraphQlFinalizerResult<>).MakeGenericType(contract), new object[] { original.AsContract(contract), finalizer });

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider) =>
            new PostResolveComplexResolverBuilder(original.ResolveComplex(serviceProvider), newResult => new GraphQlFinalizerResult<TReturnType>(newResult, finalizer));
    }
}