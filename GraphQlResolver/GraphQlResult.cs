using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    public interface IGraphQlResult
    {
        IServiceProvider ServiceProvider { get; }
        LambdaExpression UntypedResolver { get; }
        LambdaExpression? Finalizer { get; }
        IReadOnlyCollection<IGraphQlJoin> Joins { get; }
        Type ResultType { get; }
    }

    public interface IGraphQlResult<out TReturnType> : IGraphQlResult
    {
    }

    public interface IGraphQlJoin
    {
        ParameterExpression Placeholder { get; }
        LambdaExpression Conversion { get; }
    }

    public interface IGraphQlResultFactory { }

    public interface IGraphQlResultFactory<TInputType> : IGraphQlResultFactory
    {
        IGraphQlResultJoinedFactory<TInputType, TJoinedType> Join<TJoinedType>(GraphQlJoin<TInputType, TJoinedType> join);
        IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TDomainResult>> resolver);
    }

    public interface IGraphQlResultJoinedFactory<TInputType, TJoinedType>
    {
        IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TJoinedType, TDomainResult>> resolver);
    }

}
