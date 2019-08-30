using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlExpressionResult<TInput, TReturnType> : IGraphQlResult<TReturnType>
    {
        private Expression<Func<TInput, TReturnType>> func;
        private readonly IServiceProvider serviceProvider;

        public GraphQlExpressionResult(Expression<Func<TInput, TReturnType>> func, IServiceProvider serviceProvider)
        {
            this.func = func;
            this.serviceProvider = serviceProvider;
        }

        public Expression<Func<TInput1, object>> Resolve<TInput1>()
        {
            if (typeof(TInput1) != typeof(TInput))
            {
                throw new InvalidOperationException($"Expected input type of {typeof(TInput).FullName}, got {typeof(TInput1).FullName}");
            }
            // Yeah, this looks unchecked, but the if statement above fixes it
            return Expressions.ChangeReturnType<TInput1, TReturnType, object>((Expression<Func<TInput1, TReturnType>>)(Expression)func);
        }
    }

}