using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(join, serviceProvider);
        }

        IGraphQlResult<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return GraphQlExpressionResult<TDomainResult>.Construct<TValue>(resolver, serviceProvider);
        }

    }

}