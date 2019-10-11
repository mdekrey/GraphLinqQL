using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal static class GraphQlFinalizerObjectResult<TReturnType>
    {
        public static IGraphQlObjectResult<TReturnType> Inline(IGraphQlObjectResult original, LambdaExpression expressionToInline)
        {
            return new GraphQlExpressionObjectResult<TReturnType>(Inline(original.Resolution, expressionToInline), original.Contract);
        }

        private static IGraphQlScalarResult Inline(IGraphQlScalarResult original, LambdaExpression expressionToInline)
        {
            return original.UpdateBody<object>(body => Expression.Lambda(expressionToInline.Inline(body.Body), body.Parameters));
        }
    }
}