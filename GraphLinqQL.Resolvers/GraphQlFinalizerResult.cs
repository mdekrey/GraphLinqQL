using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal static class GraphQlFinalizerObjectResult<TReturnType>
    {
        public static IGraphQlObjectResult<TReturnType> Inline(IGraphQlObjectResult original, LambdaExpression expreesionToInline)
        {
            return new GraphQlExpressionObjectResult<TReturnType>(Expression.Lambda(expreesionToInline.Inline(original.UntypedResolver.Body), original.UntypedResolver.Parameters), original.Contract, original.Joins);
        }
    }
}