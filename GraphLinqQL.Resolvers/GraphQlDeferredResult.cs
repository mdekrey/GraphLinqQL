using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    class GraphQlDeferredResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        private readonly IGraphQlResult inner;
        private readonly IGraphQlResult outer;

        public GraphQlDeferredResult(
            IGraphQlResult inner,
            IGraphQlResult outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public IGraphQlParameterResolverFactory ParameterResolverFactory => outer.ParameterResolverFactory;

        public LambdaExpression UntypedResolver =>
            Expression.Lambda(ResolveDeferredExpression.Inline(outer.UntypedResolver.Body), outer.UntypedResolver.Parameters);

        public Type? Contract => inner.Contract;

        public IReadOnlyCollection<IGraphQlJoin> Joins => outer.Joins;

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            new GraphQlDeferredResult<TContract>(inner.AsContract(typeof(TContract)), outer);

        public IGraphQlResult AsContract(Type contract) =>
            (IGraphQlResult)Activator.CreateInstance(typeof(GraphQlDeferredResult<>).MakeGenericType(contract), new object[] { inner.AsContract(contract), outer });

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new PostResolveComplexResolverBuilder(inner.ResolveComplex(serviceProvider, fieldContext), newResult => new GraphQlDeferredResult<TReturnType>(newResult, outer));

        private Expression<Func<object, object?>> ResolveDeferredExpression => input => Resolve.InvokeResult(inner, input).Data;
    }
}