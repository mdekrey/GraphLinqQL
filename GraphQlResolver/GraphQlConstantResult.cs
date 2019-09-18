using System;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public static class GraphQlConstantResult
    {
        public static IGraphQlResult<TReturnType> Construct<TReturnType>(TReturnType result, IServiceProvider? serviceProvider = null)
        {
            return new GraphQlExpressionResult<TReturnType>((Expression<Func<object?, TReturnType>>)(_ => result), serviceProvider!);
        }

    }
}