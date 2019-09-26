using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    internal class GraphQlResultFactory<TValue> : GraphQlExpressionResult<TValue>, IGraphQlResultFactory<TValue>
    {
        public GraphQlResultFactory(IGraphQlParameterResolverFactory parameterResolverFactory)
            : base(parameterResolverFactory, (Expression<Func<TValue, TValue>>)(_ => _))
        {

        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(ParameterResolverFactory, join);
        }

        IGraphQlResult<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return new GraphQlExpressionResult<TDomainResult>(ParameterResolverFactory, resolver);
        }

    }

    internal static class GraphQlResultFactory
    {
        public static IGraphQlResultFactory Construct(Type modelType, IGraphQlServiceProvider serviceProvider)
        {
            return (IGraphQlResultFactory)Activator.CreateInstance(typeof(GraphQlResultFactory<>).MakeGenericType(modelType), serviceProvider.GetParameterResolverFactory())!;
        }
    }

}