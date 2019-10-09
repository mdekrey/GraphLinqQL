using System;
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

        public LambdaExpression UntypedResolver => postProcess(original.UntypedResolver);

        public Type? Contract => original.Contract;

        public bool ShouldSubselect => Contract != null;

        public IReadOnlyCollection<IGraphQlJoin> Joins => original.Joins;

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> => 
            new GraphQlFinalizerResult<TContract>(original.AsContract(typeof(TContract)), postProcess);

        public IGraphQlResult AsContract(Type contract) =>
            (IGraphQlResult)Activator.CreateInstance(typeof(GraphQlFinalizerResult<>).MakeGenericType(contract), new object[] { original.AsContract(contract), postProcess });

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new PostResolveComplexResolverBuilder(original.ResolveComplex(serviceProvider, fieldContext), newResult => new GraphQlFinalizerResult<TReturnType>(newResult, postProcess));
    }
}