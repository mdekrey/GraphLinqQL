﻿using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public static class GraphQlConstantResult
    {
        public static IGraphQlScalarResult<TReturnType> Construct<TReturnType>(TReturnType result)
        {
            return GraphQlExpressionScalarResult<TReturnType>.Constant(result);
        }

    }
}