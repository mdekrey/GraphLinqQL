using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    internal class GraphQlResultFactory<TValue> : GraphQlExpressionResult<TValue>, IGraphQlResultFactory<TValue>
    {
        public GraphQlResultFactory()
            : base((Expression<Func<TValue, TValue>>)(_ => _))
        {

        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(join);
        }

        IGraphQlResult<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return GraphQlExpressionResult<TDomainResult>.Construct(resolver);
        }

    }

}