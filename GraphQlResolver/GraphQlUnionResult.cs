using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    internal class GraphQlUnionResult<T> : IUnionGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public GraphQlUnionResult(List<IGraphQlResult<T>> allResults)
        {
            if (allResults == null || allResults.Count == 0)
            {
                throw new ArgumentException("Must provide at least one list to union.", nameof(allResults));
            }
            this.Results = allResults;
        }

        public IReadOnlyList<IGraphQlResult<T>> Results { get; }

        public IServiceProvider ServiceProvider => Results[0].ServiceProvider;

        public LambdaExpression UntypedResolver => throw new InvalidOperationException();

        public IReadOnlyCollection<IGraphQlJoin> Joins => Array.Empty<IGraphQlJoin>();

        public Type ResultType => typeof(T);
    }

    internal interface IUnionGraphQlResult<out T> : IGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public IReadOnlyList<IGraphQlResult<T>> Results { get; }
    }
}