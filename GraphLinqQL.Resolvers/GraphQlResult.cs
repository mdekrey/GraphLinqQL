using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlResult
    {
        bool ShouldSubselect { get; }
        LambdaExpression UntypedResolver { get; }
        IReadOnlyCollection<IGraphQlJoin> Joins { get; }
    }

    public interface IGraphQlResult<out TReturnType> : IGraphQlResult
    {

    }

    public interface IGraphQlScalarResult : IGraphQlResult
    {
        IGraphQlObjectResult AsContract(Type contract);
    }

    public interface IGraphQlScalarResult<out TReturnType> : IGraphQlScalarResult, IGraphQlResult<TReturnType>
    {
        IGraphQlObjectResult<TContract> AsContract<TContract>()
            where TContract : IGraphQlAccepts<TReturnType>;
    }

    public interface IGraphQlObjectResult : IGraphQlResult
    {
        Type Contract { get; }
        IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext);
    }

    public interface IGraphQlObjectResult<out TContract> : IGraphQlObjectResult, IGraphQlResult<TContract>
    {
    }

    public interface IGraphQlJoin
    {
        ParameterExpression Placeholder { get; }
        LambdaExpression Conversion { get; }
    }

#pragma warning disable CA1040 // Avoid empty interfaces - this empty interface is used to avoid reflection but still get some type safety via the IGraphQlAccepts interface
    public interface IGraphQlResultFactory { }
#pragma warning restore CA1040 // Avoid empty interfaces

    public interface IGraphQlResultFactory<TInputType> : IGraphQlResultFactory, IGraphQlScalarResult<TInputType>
    {
        IGraphQlResultJoinedFactory<TInputType, TJoinedType> Join<TJoinedType>(GraphQlJoin<TInputType, TJoinedType> join);
        IGraphQlScalarResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TDomainResult>> resolver);
    }

    public interface IGraphQlResultJoinedFactory<TInputType, TJoinedType>
    {
        IGraphQlScalarResult<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TJoinedType, TDomainResult>> resolver);
    }

}
