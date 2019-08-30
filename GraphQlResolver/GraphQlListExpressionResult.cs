using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlExpressionListResult<TInput, TReturnType> : IGraphQlResult<IEnumerable<TReturnType>>, IGraphQlResultFromInput<TInput>
    {
        private Expression<Func<TInput, IEnumerable<TReturnType>>> func;
        private readonly IServiceProvider serviceProvider;

        public GraphQlExpressionListResult(Expression<Func<TInput, IEnumerable<TReturnType>>> func, IServiceProvider serviceProvider)
        {
            this.func = func;
            this.serviceProvider = serviceProvider;
        }

        Expression<Func<TInput, object>> IGraphQlResultFromInput<TInput>.Resolve()
        {
            return Expressions.ChangeReturnType<TInput, IEnumerable<TReturnType>, object>(func);
        }
    }

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


        public IComplexResolverBuilder<TResolver, IDictionary<string, object>> As<TResolver>()
            where TResolver : IGraphQlAccepts<TReturnType>, IGraphQlResolvable
        {
            var resolver = serviceProvider.GetService<TResolver>();
            resolver.Original = new GraphQlResultFactory<TReturnType>(serviceProvider);
            return new ComplexResolverBuilder<TResolver, IDictionary<string, object>, TReturnType>(
                resolver,
                _ => new GraphQlExpressionResult<TReturnType, IDictionary<string, object>>(_, serviceProvider)
            );
        }
    }
}