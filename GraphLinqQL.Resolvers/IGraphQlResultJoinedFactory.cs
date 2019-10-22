using System;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public interface IGraphQlResultJoinedFactory<TInputType, TJoinedType>
    {
        IGraphQlScalarResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TJoinedType, TDomainResult>> resolver);
    }

}
