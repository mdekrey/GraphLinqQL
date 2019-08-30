using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    internal class GraphQlExpressionListResult<TInput, TReturnType> : IGraphQlListResult<TReturnType>, IGraphQlResultFromInput<TInput>
    {
        private Expression<Func<TInput, IEnumerable<TReturnType>>> func;

        public GraphQlExpressionListResult(Expression<Func<TInput, IEnumerable<TReturnType>>> func)
        {
            this.func = func;
        }

        Expression<Func<TInput, object>> IGraphQlResultFromInput<TInput>.Resolve()
        {
            return Expressions.ChangeReturnType<TInput, IEnumerable<TReturnType>, object>(func);
        }
    }

    internal class GraphQlExpressionResult<TInput, TReturnType> : IGraphQlResult<TReturnType>, IGraphQlResultFromInput<TInput>
    {
        private Expression<Func<TInput, TReturnType>> func;

        public GraphQlExpressionResult(Expression<Func<TInput, TReturnType>> func)
        {
            this.func = func;
        }

        Expression<Func<TInput, object>> IGraphQlResultFromInput<TInput>.Resolve()
        {
            return Expressions.ChangeReturnType<TInput, TReturnType, object>(func);
        }
    }
}