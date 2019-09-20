using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class GraphQlUnionResult<T> : IUnionGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public GraphQlUnionResult(IGraphQlParameterResolverFactory parameterResolverFactory, List<IGraphQlResult<T>> allResults)
        {
            if (allResults == null || allResults.Count == 0)
            {
                throw new ArgumentException("Must provide at least one list to union.", nameof(allResults));
            }
            this.ParameterResolverFactory = parameterResolverFactory;
            this.Results = allResults;
        }

        public IGraphQlParameterResolverFactory ParameterResolverFactory { get; }

        public IReadOnlyList<IGraphQlResult<T>> Results { get; }

        public LambdaExpression UntypedResolver => throw new InvalidOperationException();

        public LambdaExpression? Finalizer => null;
        public Type? Contract => null;

        public IReadOnlyCollection<IGraphQlJoin> Joins => Array.Empty<IGraphQlJoin>();

        public Type ResultType => typeof(T);

        public IComplexResolverBuilder ResolveComplex(IGraphQlServicesProvider serviceProvider) =>
            new UnionResolverBuilder((IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>>)this, serviceProvider);


        public IGraphQlResult As(Type contract) => throw new NotImplementedException();

        public IGraphQlResult<TContract> As<TContract>() where TContract : IGraphQlAccepts<T> =>
            (IGraphQlResult<TContract>)As(typeof(TContract));
    }

    internal interface IUnionGraphQlResult<out T> : IGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public IReadOnlyList<IGraphQlResult<T>> Results { get; }
    }
}