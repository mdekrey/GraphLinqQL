using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphLinqQL
{
    public static class GraphQlJoin
    {
        public static GraphQlJoin<TInput, IQueryable<TJoined>> JoinList<TInput, TJoined>(Expression<Func<TInput, IQueryable<TJoined>>> func)
        {
            return JoinSingle(func);
        }

        public static GraphQlJoin<TInput, TJoined> JoinSingle<TInput, TJoined>(Expression<Func<TInput, TJoined>> func)
        {
            return new GraphQlJoin<TInput, TJoined>(func);
        }
    }

    public class GraphQlJoin<TFromDomain, TToDomain> : IGraphQlJoin
    {
        private static readonly MethodInfo getJoinValue = typeof(JoinPlaceholder<TFromDomain>).GetMethod("Get").MakeGenericMethod(typeof(TToDomain));
        public ParameterExpression Placeholder { get; } = Expression.Variable(typeof(TToDomain), "JoinPlaceholder " + typeof(TToDomain).FullName);

        public Expression<Func<TFromDomain, TToDomain>> Conversion { get; }


        LambdaExpression IGraphQlJoin.Conversion => Conversion;


        public GraphQlJoin(Expression<Func<TFromDomain, TToDomain>> conversion)
        {
            Conversion = conversion;
        }
    }
}
