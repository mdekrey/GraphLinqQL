using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphLinqQL.Resolution
{
    internal class GraphQlResultFactory<TValue> : GraphQlExpressionScalarResult<TValue>, IGraphQlResultFactory<TValue>
    {
        private static readonly Expression<Func<TValue, TValue>> identity = _ => _;

        public GraphQlResultFactory(FieldContext fieldContext)
            : base(fieldContext, new[] { identity }, EmptyArrayHelper.Empty<ExpressionVisitor>(), EmptyArrayHelper.Empty<IGraphQlJoin>())
        {

        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(FieldContext, join);
        }

        IGraphQlScalarResult<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return GraphQlExpressionScalarResult<TDomainResult>.Simple(FieldContext, resolver);
        }

    }
}
