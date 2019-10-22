using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphLinqQL
{
    public class GraphQlJoin<TFromDomain, TToDomain> : IGraphQlJoin
    {
        public ParameterExpression Placeholder { get; } = Expression.Variable(typeof(TToDomain), "JoinPlaceholder " + typeof(TToDomain).FullName);

        public Expression<Func<TFromDomain, TToDomain>> Conversion { get; }


        LambdaExpression IGraphQlJoin.Conversion => Conversion;


        public GraphQlJoin(Expression<Func<TFromDomain, TToDomain>> conversion)
        {
            Conversion = conversion;
        }
    }
}
