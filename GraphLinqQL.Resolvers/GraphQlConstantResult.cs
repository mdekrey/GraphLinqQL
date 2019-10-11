using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public static class GraphQlConstantResult
    {
        public static IGraphQlScalarResult<TReturnType> Construct<TReturnType>(TReturnType result)
        {
            return new GraphQlExpressionScalarResult<TReturnType>((Expression<Func<object?, TReturnType>>)(_ => result), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

    }
}