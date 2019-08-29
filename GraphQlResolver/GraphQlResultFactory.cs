using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlResultFactory<TValue> : IGraphQlResultFactory<TValue>
    {
        private readonly IServiceProvider serviceProvider;
        public GraphQlResultFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            throw new System.NotImplementedException();
        }

        IGraphQlResultWithComplexFactory<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return new GraphQlResultResolving<TValue, TDomainResult>(serviceProvider, resolver);
        }

        IGraphQlListResultWithComplexFactory<TModel> IGraphQlResultFactory<TValue>.ResolveList<TModel>(Expression<Func<TValue, IEnumerable<TModel>>> resolver)
        {
            return new GraphQlListResultResolving<TValue, TModel>(serviceProvider, resolver);
        }
    }

    internal class GraphQlResultResolving<TValue, TModel> : IGraphQlResultWithComplexFactory<TModel>
    {
        private Expression<Func<TValue, TModel>> resolver;
        private readonly IServiceProvider serviceProvider;

        public GraphQlResultResolving(IServiceProvider serviceProvider, Expression<Func<TValue, TModel>> resolver)
        {
            this.serviceProvider = serviceProvider;
            this.resolver = resolver;
        }

        IGraphQlComplexResult<TContract> IGraphQlResultWithComplexFactory<TModel>.As<TContract>()
        {
            throw new NotImplementedException();
        }
    }

    internal class GraphQlListResultResolving<TInput, TModel> : IGraphQlListResultWithComplexFactory<TModel>
    {
        private readonly Expression<Func<TInput, IEnumerable<TModel>>> resolver;
        private readonly IServiceProvider serviceProvider;

        public GraphQlListResultResolving(IServiceProvider serviceProvider, Expression<Func<TInput, IEnumerable<TModel>>> resolver)
        {
            this.serviceProvider = serviceProvider;
            this.resolver = resolver;
        }

        IGraphQlComplexListResult<TContract> IGraphQlListResultWithComplexFactory<TModel>.As<TContract>()
        {
            return new GraphQlExpressionResult<TContract, TInput, TModel>(resolver, serviceProvider);
        }
    }

    internal class GraphQlExpressionResult<TContract, TInput, TModel> : IGraphQlComplexListResult<TContract>
        where TContract : IGraphQlResolvable, IGraphQlAccepts<TModel>
    {
        private readonly Expression<Func<TInput, IEnumerable<TModel>>> resolver;
        private readonly IServiceProvider serviceProvider;

        public GraphQlExpressionResult(Expression<Func<TInput, IEnumerable<TModel>>> resolver, IServiceProvider serviceProvider)
        {
            this.resolver = resolver;
            this.serviceProvider = serviceProvider;
        }

        IComplexResolverBuilder<TContract, IGraphQlListResult<IDictionary<string, object>>> IGraphQlComplexListResult<TContract>.ResolveComplex()
        {
            var resolver = serviceProvider.GetService<TContract>();
            resolver.Original = new GraphQlResultFactory<TModel>(serviceProvider);
            return new ComplexResolverBuilder<TContract, IGraphQlListResult<IDictionary<string, object>>, TModel>(
                resolver,
                _ =>
                {
                    var topResolver = this.resolver;
                    return null; // TODO
                }
            );
        }
    }
}