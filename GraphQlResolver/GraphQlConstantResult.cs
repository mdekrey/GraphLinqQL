using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GraphQlSchema;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlConstantResult<TValue> : IGraphQlResult<TValue>
    {
        private TValue value;
        private readonly IServiceProvider serviceProvider;

        public GraphQlConstantResult(TValue value, IServiceProvider serviceProvider)
        {
            this.value = value;
            this.serviceProvider = serviceProvider;
        }

    }

    internal class GraphQlConstantResult<TValue, TResolver> : IGraphQlComplexResult<TResolver>, IGraphQlResult<TValue>
            where TResolver : IGraphQlAccepts<TValue>, IGraphQlResolvable
    {
        private TValue value;
        private readonly IServiceProvider serviceProvider;

        public GraphQlConstantResult(TValue value, IServiceProvider serviceProvider)
        {
            this.value = value;
            this.serviceProvider = serviceProvider;
        }

        IComplexResolverBuilder<TResolver, IGraphQlResult<IDictionary<string, object>>> IGraphQlComplexResult<TResolver>.ResolveComplex()
        {
            var resolver = serviceProvider.GetService<TResolver>();
            resolver.Original = new GraphQlResultFactory<TValue>(serviceProvider);
            return new ComplexResolverBuilder<TResolver, IGraphQlResult<IDictionary<string, object>>, TValue>(
                resolver, 
                _ => new GraphQlExpressionResult<TValue, IDictionary<string, object>>(_)
            );
        }
    }
}