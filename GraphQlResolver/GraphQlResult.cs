using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    public interface IGraphQlResult
    {
    }

    public interface IGraphQlResultFromInput<TInput>
    {
        Expression<Func<TInput, object>> Resolve();
    }

    public interface IGraphQlResult<out TReturnType> : IGraphQlResult
    {
    }

    public interface IGraphQlListResult<out TReturnType> : IGraphQlResult
    {
    }

    public interface IGraphQlComplexResult<out TReturnType> : IGraphQlResult
        where TReturnType : IGraphQlResolvable
    {
        IComplexResolverBuilder<TReturnType, IGraphQlResult<IDictionary<string, object>>> ResolveComplex();
    }

    public interface IGraphQlComplexListResult<out TReturnType> : IGraphQlResult
        where TReturnType : IGraphQlResolvable
    {
        IComplexResolverBuilder<TReturnType, IGraphQlListResult<IDictionary<string, object>>> ResolveComplex();
    }

    public interface IGraphQlResultFactory
    {
    }

    public interface IGraphQlResultFactory<TInputType>
    {
        IGraphQlResultJoinedFactory<TInputType, TJoinedType> Join<TJoinedType>(GraphQlJoin<TInputType, TJoinedType> join);
        IGraphQlListResultWithComplexFactory<TDomainResult> ResolveList<TDomainResult>(Expression<Func<TInputType, IEnumerable<TDomainResult>>> resolver);
        IGraphQlResultWithComplexFactory<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TDomainResult>> resolver);
    }

    public interface IGraphQlResultJoinedFactory<TInputType, TJoinedType>
    {
        IGraphQlListResultWithComplexFactory<TDomainResult> ResolveList<TDomainResult>(Expression<Func<TInputType, TJoinedType, IEnumerable<TDomainResult>>> resolver);
        IGraphQlResultWithComplexFactory<TDomainResult> Resolve<TDomainResult>(Expression<Func<TInputType, TJoinedType, TDomainResult>> resolver);
    }

    public interface IGraphQlListResultWithComplexFactory<TModel> : IGraphQlListResult<TModel>
    {
        IGraphQlComplexListResult<TContract> As<TContract>()
            where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable;
    }

    public interface IGraphQlResultWithComplexFactory<TModel> : IGraphQlResult<TModel>
    {
        IGraphQlComplexResult<TContract> As<TContract>()
            where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable;
    }

}
