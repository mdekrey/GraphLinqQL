using System;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public static class ResolveNullabilityExtensions
    {
        public static IGraphQlScalarResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            throw new NotImplementedException();
            //var newResult = func(new GraphQlResultFactory<TInput>());

            //return new GraphQlExpressionScalarResult<TContract?>(NullableCheck(newResult.UntypedResolver, original.UntypedResolver), newResult.Joins);
        }

        public static IGraphQlObjectResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
            where TInput : class
            where TContract : class
        {

            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdateBody<object>(getListLamba =>
            {
                return NullableCheck(constructedDeferred, getListLamba);
            });
            return newResult.AdjustResolution<TContract?>(_ => newScalar);
        }

        private static LambdaExpression NullableCheck(LambdaExpression newResultResolver, LambdaExpression original)
        {
            return newResultResolver;
            //return Expression.Lambda(original.Body.IfNotNull(newResultResolver.Inline(original.Body)), original.Parameters);
        }
    }
}
