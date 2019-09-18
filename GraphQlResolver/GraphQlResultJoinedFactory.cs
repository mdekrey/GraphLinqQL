using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    internal class GraphQlResultJoinedFactory<TValue, TJoinedType> : IGraphQlResultJoinedFactory<TValue, TJoinedType>
    {
        private GraphQlJoin<TValue, TJoinedType> join;

        public GraphQlResultJoinedFactory(GraphQlJoin<TValue, TJoinedType> join)
        {
            this.join = join;
        }

        public IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TValue, TJoinedType, TDomainResult>> resolver)
        {
            var newFunc = Expression.Lambda<Func<TValue, TDomainResult>>(resolver.Body.Replace(resolver.Parameters[1], join.Placeholder), resolver.Parameters[0]);
            return new GraphQlExpressionResult<TDomainResult>(newFunc, ImmutableHashSet.Create<IGraphQlJoin>(join));
        }
    }
}