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


        public IComplexResolverBuilder<TContract, IDictionary<string, object>> As<TContract>()
            where TContract : IGraphQlAccepts<TReturnType>, IGraphQlResolvable
        {
            var resolver = serviceProvider.GetService<TContract>();
            resolver.Original = new GraphQlResultFactory<TReturnType>(serviceProvider);
            return new ComplexResolverBuilder<TContract, IDictionary<string, object>, TReturnType>(
                resolver,
                ToResult
            );
        }

        private IGraphQlResult<IDictionary<string, object>> ToResult(Expression<Func<TReturnType, IDictionary<string, object>>> expression)
        {
            return new GraphQlExpressionResult<TReturnType, IDictionary<string, object>>(expression, serviceProvider);
        }
    }

}