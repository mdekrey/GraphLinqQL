using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    public static class GraphQlJoin
    {
        public static GraphQlJoin<TInput, TJoined> Join<TInput, TJoined>(Func<IQueryable<TInput>, IQueryable<TJoined>> func)
        {
            var root = Enumerable.Empty<TInput>().AsQueryable();
            return new GraphQlJoin<TInput, TJoined>(func(root), root);
        }
    }

    public class GraphQlJoin<TFromDomain, TToDomain> : IGraphQlJoin
    {
        public ParameterExpression Placeholder { get; } = Expression.Variable(typeof(TToDomain));

        public IQueryable<TToDomain> Queryable { get; }
        public IQueryable<TFromDomain> Root { get; }


        IQueryable IGraphQlJoin.Queryable => Queryable;
        IQueryable IGraphQlJoin.Root => Root;

        public GraphQlJoin(IQueryable<TToDomain> queryable, IQueryable<TFromDomain> root)
        {
            this.Queryable = queryable;
            this.Root = root;
        }
    }
}
