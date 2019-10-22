using GraphLinqQL.Resolution;
using System;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public static class ResolveNullabilityExtensions
    {
        public static IGraphQlScalarResult<TContract> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
            where TInput : class
        {
            var newResult = func(new GraphQlResultFactory<TInput>(original.FieldContext));
            var constructedDeferred = newResult.ConstructResult();

            var newScalar = original.UpdateBody<TContract>(
                body =>
                {
                    var withNullCheck = NullableCheck(constructedDeferred, body);
                    return withNullCheck;
                });
            return newScalar.Catch();
        }

        public static IGraphQlObjectResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            var newResult = func(new GraphQlResultFactory<TInput>(original.FieldContext));
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdateBody<object>(
                getListLamba =>
            {
                var withNullCheck = NullableCheck(constructedDeferred, getListLamba);
                return withNullCheck;
            }).Catch();
            return newResult.AdjustResolution<TContract?>(_ => newScalar);
        }

        private static LambdaExpression NullableCheck(LambdaExpression newResultResolver, LambdaExpression original)
        {
            return Expression.Lambda(original.Body.IfNotNull(newResultResolver.Inline(original.Body)), original.Parameters);
        }

        public static IGraphQlScalarResult<T> Catch<T>(this IGraphQlScalarResult<T> original)
        {
            var catchFinalizerFactory = new CatchFinalizerFactory(original.FieldContext);

            return original.UpdateBody<T>(body => Expression.Lambda(Expression.Call(Expression.Constant(catchFinalizerFactory), CatchFinalizerFactory.CatchMethodInfo, Expression.Lambda<Func<T>>(body.Body)), body.Parameters));
        }
    }
}
