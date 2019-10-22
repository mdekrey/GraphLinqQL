using System;
using System.Linq.Expressions;

namespace GraphLinqQL
{

    public interface IGraphQlResultFactory<TInputType> : IGraphQlResultFactory, IGraphQlScalarResult<TInputType>
    {
        IGraphQlResultJoinedFactory<TInputType, TJoinedType> Join<TJoinedType>(GraphQlJoin<TInputType, TJoinedType> join);
        IGraphQlScalarResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TDomainResult>> resolver);
    }

}
