using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQlResolver
{
    public static class GraphQlJoin
    {
        public static GraphQlJoin<TInput, TJoined> Join<TInput, TJoined>(Func<IQueryable<TInput>, IQueryable<TJoined>> func)
        {
            var root = Resolve.Query<TInput>();
            return new GraphQlJoin<TInput, TJoined>(func(root), root);
        }
    }

    public class GraphQlJoin<TFromDomain, TToDomain>
    {
        private IQueryable<TToDomain> queryable;
        private IQueryable<TFromDomain> root;

        public GraphQlJoin(IQueryable<TToDomain> queryable, IQueryable<TFromDomain> root)
        {
            this.queryable = queryable;
            this.root = root;
        }
    }
}
