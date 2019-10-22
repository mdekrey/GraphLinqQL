using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public static class GraphQlAcceptsExtension
    {
        public static IGraphQlResultFactory<T> Original<T>(this IGraphQlAccepts<T> accepts)
        {
            return (IGraphQlResultFactory<T>)accepts.Original;
        }

        public static IGraphQlResultJoinedFactory<TInputType, TJoinedType> Join<TInputType, TJoinedType>(this IGraphQlAccepts<TInputType> accepts, GraphQlJoin<TInputType, TJoinedType> join)
        {
            return accepts.Original().Join(join);
        }

        public static IGraphQlScalarResult<TDomainResult> Resolve<TInputType, TDomainResult>(this IGraphQlAccepts<TInputType> accepts, Expression<Func<TInputType, TDomainResult>> resolver)
        {
            return accepts.Original().Resolve(resolver);
        }

        public static IGraphQlScalarResult<TDomainResult> Resolve<TInputType, TDomainResult>(this IGraphQlAccepts<TInputType> accepts, TDomainResult result)
        {
            return accepts.Original().Resolve(result);
        }

        public static IGraphQlObjectResult<IEnumerable<TContractResult>> Union<TInputType, TContractResult>(this IGraphQlAccepts<TInputType> accepts, params Func<IGraphQlResultFactory<TInputType>, IGraphQlObjectResult<IEnumerable<TContractResult>>>[] funcs)
            where TContractResult : IGraphQlResolvable?
        {
            return accepts.Original().Union(funcs);
        }

        public static IGraphQlScalarResult<TDomainResult> ResolveTask<TInputType, TDomainResult>(this IGraphQlAccepts<TInputType> accepts, Func<TInputType, Task<TDomainResult>> resolveAsync)
        {
            return accepts.Original().ResolveTask(resolveAsync);
        }
    }
}