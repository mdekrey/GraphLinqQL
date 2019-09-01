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
        IReadOnlyCollection<IGraphQlJoin> Joins { get; }
    }

    public interface IGraphQlResult<out TReturnType> : IGraphQlResult
    {
    }

    public interface IGraphQlJoin
    {
        ParameterExpression Placeholder { get; }
        Expression Queryable { get; }
        Expression Root { get; }
        Expression Convert(ParameterExpression joinPlaceholderParameter);
        Expression GetAccessor(ParameterExpression joinPlaceholderParameter);
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
