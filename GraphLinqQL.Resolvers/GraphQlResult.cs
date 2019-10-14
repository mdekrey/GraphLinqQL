﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL
{
#pragma warning disable CA1040 // Avoid empty interfaces - this is indicated as a Union type, essentially.
    public interface IGraphQlResult
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }

    public interface IGraphQlScalarResult : IGraphQlResult
    {
        LambdaExpression ConstructResult();

        // FIXME - if we can not expose these as interface members it would be better
        LambdaExpression Body { get; }

        IReadOnlyCollection<IGraphQlJoin> Joins { get; }
        IGraphQlObjectResult<T> AsContract<T>(IContract contract, Func<Expression, Expression> bodyWrapper);

        // TODO - should prefer this to preamble/body
        //IGraphQlScalarResult<T> UpdateCurrent<T>(Func<LambdaExpression, LambdaExpression> resolveAdjust);
        IGraphQlScalarResult<T> UpdatePreamble<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust);
        IGraphQlScalarResult<T> UpdateBody<T>(Func<LambdaExpression, LambdaExpression> bodyAdjust);
        IGraphQlScalarResult<T> UpdatePreambleAndBody<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust, Func<LambdaExpression, LambdaExpression> bodyAdjust);
    }

    public interface IGraphQlScalarResult<out TReturnType> : IGraphQlScalarResult, IGraphQlResult
    {
        IGraphQlObjectResult<TContract> AsContract<TContract>()
            where TContract : IGraphQlAccepts<TReturnType>;
    }

    public interface IGraphQlObjectResult : IGraphQlResult
    {
        IGraphQlScalarResult Resolution { get; }
        IContract Contract { get; }

        IGraphQlObjectResult<T> AdjustResolution<T>(Func<IGraphQlScalarResult, IGraphQlScalarResult> p);
        IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext);
    }

    public interface IGraphQlObjectResult<out TContract> : IGraphQlObjectResult, IGraphQlResult
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
