namespace GraphQlResolver
{
    internal class GraphQlResultJoinedFactory<TValue, TJoinedType> : IGraphQlResultJoinedFactory<TValue, TJoinedType>
    {
        private GraphQlJoin<TValue, TJoinedType> join;

        public GraphQlResultJoinedFactory(GraphQlJoin<TValue, TJoinedType> join)
        {
            this.join = join;
        }

        public IGraphQlResultWithComplexFactory<TDomainResult> Resolve<TDomainResult>(System.Linq.Expressions.Expression<System.Func<TValue, TJoinedType, TDomainResult>> resolver)
        {
            throw new System.NotImplementedException();
        }

        public IGraphQlListResultWithComplexFactory<TDomainResult> ResolveList<TDomainResult>(System.Linq.Expressions.Expression<System.Func<TValue, TJoinedType, System.Collections.Generic.IEnumerable<TDomainResult>>> resolver)
        {
            throw new System.NotImplementedException();
        }
    }
}