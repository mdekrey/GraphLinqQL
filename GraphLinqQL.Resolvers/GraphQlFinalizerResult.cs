using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal static class GraphQlFinalizerObjectResult<TReturnType>
    {
        public static IGraphQlObjectResult<TReturnType> Inline(IGraphQlObjectResult original, LambdaExpression expressionToInline)
        {
            return original.AdjustResolution<TReturnType>(resolution => Inline(resolution, expressionToInline));
        }

        private static IGraphQlScalarResult Inline(IGraphQlScalarResult original, LambdaExpression expressionToInline)
        {
            return original.AddResolve<object>(expressionToInline);
        }
    }
}