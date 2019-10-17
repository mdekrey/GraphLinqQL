using System;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public static class ResolveNullabilityExtensions
    {
        public static IGraphQlObjectResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdateBody<object>(
                getListLamba =>
            {
                var withNullCheck = NullableCheck(constructedDeferred, getListLamba);
                return withNullCheck;
            });
            return newResult.AdjustResolution<TContract?>(_ => newScalar);
        }

        private static LambdaExpression NullableCheck(LambdaExpression newResultResolver, LambdaExpression original)
        {
            return Expression.Lambda(original.Body.IfNotNull(newResultResolver.Inline(original.Body)), original.Parameters);
        }
    }
}
