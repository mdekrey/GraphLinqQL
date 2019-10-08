using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
#if NETFRAMEWORK
    internal static class EmptyJoinArrayContainer
    {
        public static readonly IReadOnlyCollection<IGraphQlJoin> Joins = new IGraphQlJoin[0];
    }
#endif

    internal class GraphQlUnionResult<T> : IUnionGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public GraphQlUnionResult(IGraphQlParameterResolverFactory parameterResolverFactory, List<IGraphQlResult<T>> allResults)
        {
            if (allResults == null || allResults.Count == 0)
            {
                throw new ArgumentException("Must provide at least one result of a list to union.", nameof(allResults));
            }
            this.ParameterResolverFactory = parameterResolverFactory;
            this.Results = allResults;
        }

        public IGraphQlParameterResolverFactory ParameterResolverFactory { get; }

        public IReadOnlyList<IGraphQlResult<T>> Results { get; }

        public LambdaExpression UntypedResolver => throw new InvalidOperationException();

        public Type? Contract => null;

#if NETFRAMEWORK
        public IReadOnlyCollection<IGraphQlJoin> Joins => EmptyJoinArrayContainer.Joins;
#else
        public IReadOnlyCollection<IGraphQlJoin> Joins => Array.Empty<IGraphQlJoin>();
#endif

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new UnionResolverBuilder((IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>>)this, serviceProvider, fieldContext);


        public IGraphQlResult AsContract(Type contract) => throw new NotSupportedException();

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<T> =>
            (IGraphQlResult<TContract>)AsContract(typeof(TContract));
    }

    internal interface IUnionGraphQlResult<out T> : IGraphQlResult<T>
        where T : IEnumerable<IGraphQlResolvable?>?
    {
        public IReadOnlyList<IGraphQlResult<T>> Results { get; }
    }
}