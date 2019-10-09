using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class GraphQlUnionResult<T> : IUnionGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public GraphQlUnionResult(List<IGraphQlObjectResult<T>> allResults)
        {
            if (allResults == null || allResults.Count == 0)
            {
                throw new ArgumentException("Must provide at least one result of a list to union.", nameof(allResults));
            }
            this.Results = allResults;
        }

        public IReadOnlyList<IGraphQlObjectResult<T>> Results { get; }

        public LambdaExpression UntypedResolver => throw new InvalidOperationException();

        public Type Contract => null!; // TODO - this could be implemented via an interface!?

        public bool ShouldSubselect => true;

        public IReadOnlyCollection<IGraphQlJoin> Joins => EmptyArrayHelper.Empty<IGraphQlJoin>();

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new UnionResolverBuilder((IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>>)this, serviceProvider, fieldContext);
    }

    internal interface IUnionGraphQlResult<out T> : IGraphQlObjectResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public IReadOnlyList<IGraphQlObjectResult<T>> Results { get; }
    }
}