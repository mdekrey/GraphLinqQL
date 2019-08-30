using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlExpressionResult<TInput, TReturnType> : IGraphQlResult<TReturnType>, IGraphQlResultFromInput<TInput>
    {
        private Expression<Func<TInput, TReturnType>> func;
        private readonly IServiceProvider serviceProvider;

        public GraphQlExpressionResult(Expression<Func<TInput, TReturnType>> func, IServiceProvider serviceProvider)
        {
            this.func = func;
            this.serviceProvider = serviceProvider;
        }

        Expression<Func<TInput, object>> IGraphQlResultFromInput<TInput>.Resolve()
        {
            return Expressions.ChangeReturnType<TInput, TReturnType, object>(func);
        }
    }

}