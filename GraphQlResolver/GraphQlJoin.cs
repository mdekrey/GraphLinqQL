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
            return null;
        }
    }

    public class GraphQlJoin<TFromDomain, TToDomain>
    {
    }
}
