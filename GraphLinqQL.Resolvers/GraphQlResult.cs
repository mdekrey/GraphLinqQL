﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlResult
    {
        LambdaExpression UntypedResolver { get; }
        Type? Contract { get; }
        bool ShouldSubselect { get; }
        IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext queryContext);

        IGraphQlResult AsContract(Type contract);
    }

    public interface IGraphQlResult<out TReturnType> : IGraphQlResult
    {
        IGraphQlResult<TContract> AsContract<TContract>()
            where TContract : IGraphQlAccepts<TReturnType>;
    }

    public interface IGraphQlJoin
    {
        ParameterExpression Placeholder { get; }
        LambdaExpression Conversion { get; }
    }

#pragma warning disable CA1040 // Avoid empty interfaces - this empty interface is used to avoid reflection but still get some type safety via the IGraphQlAccepts interface
    public interface IGraphQlResultFactory { }
#pragma warning restore CA1040 // Avoid empty interfaces

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
