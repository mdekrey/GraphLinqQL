using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    internal class GraphQlResultJoinedFactory<TValue, TJoinedType> : IGraphQlResultJoinedFactory<TValue, TJoinedType>
    {
        private GraphQlJoin<TValue, TJoinedType> join;
        private readonly IServiceProvider serviceProvider;

        public GraphQlResultJoinedFactory(GraphQlJoin<TValue, TJoinedType> join, IServiceProvider serviceProvider)
        {
            this.join = join;
            this.serviceProvider = serviceProvider;
        }

        public IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TValue, TJoinedType, TDomainResult>> resolver)
        {
            var newFunc = Expression.Lambda<Func<TValue, TDomainResult>>(resolver.Body.Replace(resolver.Parameters[1], join.Placeholder), resolver.Parameters[0]);
            return new GraphQlExpressionResult<TDomainResult>(newFunc, serviceProvider, ImmutableHashSet.Create<IGraphQlJoin>(join));
        }
    }
}