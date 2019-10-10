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
            var newResult = func(new GraphQlResultFactory<TInput>());

            return new GraphQlExpressionScalarResult<TContract?>(NullableCheck(newResult.UntypedResolver, original.UntypedResolver), newResult.Joins);
        }

        public static IGraphQlObjectResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            return new GraphQlExpressionObjectResult<TContract>(NullableCheck(newResult.UntypedResolver, original.UntypedResolver), newResult.Contract, newResult.Joins);
        }

        private static LambdaExpression NullableCheck(LambdaExpression newResultResolver, LambdaExpression original)
        {
            return Expression.Lambda(original.Body.IfNotNull(newResultResolver.Inline(original.Body)), original.Parameters);
        }
    }
}
