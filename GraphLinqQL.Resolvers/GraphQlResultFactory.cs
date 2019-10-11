using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    internal class GraphQlResultFactory<TValue> : GraphQlExpressionScalarResult<TValue>, IGraphQlResultFactory<TValue>
    {
        public GraphQlResultFactory()
            : base((Expression<Func<TValue, TValue>>)(_ => _), (Expression<Func<TValue, TValue>>)(_ => _), System.Collections.Immutable.ImmutableHashSet<IGraphQlJoin>.Empty)
        {

        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(join);
        }

        IGraphQlScalarResult<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return GraphQlExpressionScalarResult<TDomainResult>.Simple(resolver);
        }

    }

    internal static class GraphQlResultFactory
    {
        public static IGraphQlResultFactory Construct(Type modelType)
        {
            return (IGraphQlResultFactory)Activator.CreateInstance(typeof(GraphQlResultFactory<>).MakeGenericType(modelType))!;
        }
    }

}