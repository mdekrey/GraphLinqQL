using System;

namespace GraphQlResolver
{
    internal class GraphQlResultJoinedFactory<TValue, TJoinedType> : IGraphQlResultJoinedFactory<TValue, TJoinedType>
    {
        private GraphQlJoin<TValue, TJoinedType> join;

        public GraphQlResultJoinedFactory(GraphQlJoin<TValue, TJoinedType> join)
        {
            this.join = join;
        }

        public IGraphQlResult<TDomainResult> Resolve<TDomainResult>(System.Linq.Expressions.Expression<System.Func<TValue, TJoinedType, TDomainResult>> resolver)
        {
            throw new NotImplementedException();
        }
    }
}