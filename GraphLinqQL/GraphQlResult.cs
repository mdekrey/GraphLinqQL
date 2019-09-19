using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlResult
    {
        IGraphQlParameterResolverFactory ParameterResolverFactory { get; }
        LambdaExpression UntypedResolver { get; }
        LambdaExpression? Finalizer { get; }
        Type? Contract { get; }
        IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        IComplexResolverBuilder ResolveComplex(IServiceProvider serviceProvider);

        IGraphQlResult As(Type contract);
    }

    public interface IGraphQlResult<out TReturnType> : IGraphQlResult
    {
        IGraphQlResult<TContract> As<TContract>()
            where TContract : IGraphQlAccepts<TReturnType>;
    }

    public interface IGraphQlJoin
    {
        ParameterExpression Placeholder { get; }
        LambdaExpression Conversion { get; }
    }

    public interface IGraphQlResultFactory { }

    public interface IGraphQlResultFactory<TInputType> : IGraphQlResultFactory, IGraphQlResult<TInputType>
    {
        IGraphQlResultJoinedFactory<TInputType, TJoinedType> Join<TJoinedType>(GraphQlJoin<TInputType, TJoinedType> join);
        IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TDomainResult>> resolver);
    }

    public interface IGraphQlResultJoinedFactory<TInputType, TJoinedType>
    {
        IGraphQlResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TJoinedType, TDomainResult>> resolver);
    }

}
