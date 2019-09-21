using System;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public static class GraphQlConstantResult
    {
        public static IGraphQlResult<TReturnType> Construct<TReturnType>(TReturnType result)
        {
            return new GraphQlExpressionResult<TReturnType>(null!, (Expression<Func<object?, TReturnType>>)(_ => result));
        }

    }
}