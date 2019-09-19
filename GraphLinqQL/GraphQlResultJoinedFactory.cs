using System;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class GraphQlResultJoinedFactory<TValue, TJoinedType> : IGraphQlResultJoinedFactory<TValue, TJoinedType>
    {
        private readonly IGraphQlParameterResolverFactory parameterResolverFactory;
        private GraphQlJoin<TValue, TJoinedType> join;

        public GraphQlResultJoinedFactory(IGraphQlParameterResolverFactory parameterResolverFactory, GraphQlJoin<TValue, TJoinedType> join)
        {
            this.parameterResolverFactory = parameterResolverFactory;
            this.join = join;
        }

        public IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TValue, TJoinedType, TDomainResult>> resolver)
        {
            var newFunc = Expression.Lambda<Func<TValue, TDomainResult>>(resolver.Body.Replace(resolver.Parameters[1], join.Placeholder), resolver.Parameters[0]);
            return new GraphQlExpressionResult<TDomainResult>(parameterResolverFactory, newFunc, null, ImmutableHashSet.Create<IGraphQlJoin>(join));
        }
    }
}