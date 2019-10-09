using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class GraphQlFinalizerScalarResult<TReturnType> : IGraphQlScalarResult<TReturnType>
    {
        private readonly IGraphQlScalarResult original;
        private readonly Func<LambdaExpression, LambdaExpression> postProcess;

        public GraphQlFinalizerScalarResult(IGraphQlScalarResult original, Func<LambdaExpression, LambdaExpression> postProcess)
        {
            this.original = original;
            this.postProcess = postProcess;
        }

        public bool ShouldSubselect => false;

        public LambdaExpression UntypedResolver => postProcess(original.UntypedResolver);

        public IReadOnlyCollection<IGraphQlJoin> Joins => original.Joins;

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            new GraphQlFinalizerObjectResult<TContract>(original.AsContract(typeof(TContract)), postProcess);

        public IGraphQlObjectResult AsContract(Type contract) =>
            (IGraphQlObjectResult)Activator.CreateInstance(typeof(GraphQlFinalizerObjectResult<>).MakeGenericType(contract), new object[] { original.AsContract(contract), postProcess });

    }
    internal class GraphQlFinalizerObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly IGraphQlObjectResult original;
        private readonly Func<LambdaExpression, LambdaExpression> postProcess;

        public GraphQlFinalizerObjectResult(IGraphQlObjectResult original, Func<LambdaExpression, LambdaExpression> postProcess)
        {
            this.original = original;
            this.postProcess = postProcess;
        }

        public static IGraphQlObjectResult<TReturnType> Inline(IGraphQlObjectResult original, LambdaExpression expreesionToInline)
        {
            return new GraphQlFinalizerObjectResult<TReturnType>(original, original => Expression.Lambda(expreesionToInline.Inline(original.Body), original.Parameters));
        }

        public Type Contract => original.Contract;

        public bool ShouldSubselect => true;

        public LambdaExpression UntypedResolver => postProcess(original.UntypedResolver);

        public IReadOnlyCollection<IGraphQlJoin> Joins => original.Joins;

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new PostResolveComplexResolverBuilder(original.ResolveComplex(serviceProvider, fieldContext), newResult =>
                new GraphQlExpressionScalarResult<object>(postProcess(newResult.UntypedResolver), newResult.Joins));

    }
}