using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class MultiObjectResult<TContractResult> : IGraphQlObjectResult<TContractResult>
    {
        private IReadOnlyList<IGraphQlObjectResult<TContractResult>> objectResults;
        private readonly Func<IReadOnlyList<LambdaExpression>, LambdaExpression> combineResults;

        public MultiObjectResult(IGraphQlObjectResult<TContractResult>[] objectResults, Func<IReadOnlyList<LambdaExpression>, LambdaExpression> combineResults)
        {
            this.objectResults = objectResults;
            this.combineResults = combineResults;
        }

        public IGraphQlScalarResult Resolution => throw new NotImplementedException();

        public IGraphQlObjectResult<T> AdjustResolution<T>(Func<IGraphQlScalarResult, IGraphQlScalarResult> p)
        {
            return new MultiObjectResult<T>(objectResults.Select(r => r.AdjustResolution<T>(p)).ToArray(), combineResults);
        }

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext)
        {
            return new MultiObjectComplexResolver(objectResults.Select(r => r.ResolveComplex(serviceProvider, fieldContext)).ToArray(), combineResults, objectResults.SelectMany(r => r.Resolution.Joins).Distinct().ToArray());
        }
    }
}