using GraphLinqQL.Resolution;
using System;

namespace GraphLinqQL
{
    public interface IGraphQlObjectResult : IGraphQlResult
    {
        IGraphQlScalarResult Resolution { get; }
        IContract Contract { get; }

        IGraphQlObjectResult<T> AdjustResolution<T>(Func<IGraphQlScalarResult, IGraphQlScalarResult> p);
        IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider);
    }

}
