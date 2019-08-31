using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    class GraphQlExpressionResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        public IServiceProvider ServiceProvider { get; }

        public LambdaExpression UntypedResolver { get; }

        public GraphQlExpressionResult(LambdaExpression func, IServiceProvider serviceProvider)
        {
            this.UntypedResolver = func;
            this.ServiceProvider = serviceProvider;
        }

        public static GraphQlExpressionResult<TReturnType> Construct<TInput>(Expression<Func<TInput, TReturnType>> func, IServiceProvider serviceProvider)
        {
            return new GraphQlExpressionResult<TReturnType>(func, serviceProvider);
        }
    }

}