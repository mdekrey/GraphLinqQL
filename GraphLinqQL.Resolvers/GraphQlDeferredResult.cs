using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    class GraphQlDeferredScalarResult<TReturnType> : IGraphQlScalarResult<TReturnType>
    {
        private readonly IGraphQlScalarResult inner;
        private readonly IGraphQlScalarResult outer;

        public GraphQlDeferredScalarResult(
            IGraphQlScalarResult inner,
            IGraphQlScalarResult outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public LambdaExpression UntypedResolver =>
            Expression.Lambda(ResolveDeferredExpression.Inline(outer.UntypedResolver.Body), outer.UntypedResolver.Parameters);

        public IReadOnlyCollection<IGraphQlJoin> Joins => outer.Joins;

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            new GraphQlDeferredObjectResult<TContract>(inner.AsContract(typeof(TContract)), outer);

        public IGraphQlObjectResult AsContract(Type contract) =>
            (IGraphQlObjectResult)Activator.CreateInstance(typeof(GraphQlDeferredObjectResult<>).MakeGenericType(contract), new object[] { inner.AsContract(contract), outer });

        private Expression<Func<object, object?>> ResolveDeferredExpression => input => Execution.GraphQlResultExtensions.InvokeResult(inner, input).Data;
    }

    class GraphQlDeferredObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly IGraphQlObjectResult inner;
        private readonly IGraphQlScalarResult initial;

        public GraphQlDeferredObjectResult(
            IGraphQlObjectResult inner,
            IGraphQlScalarResult initial)
        {
            this.inner = inner;
            this.initial = initial;
        }

        public LambdaExpression UntypedResolver =>
            Expression.Lambda(ResolveDeferredExpression.Inline(initial.UntypedResolver.Body), initial.UntypedResolver.Parameters);

        public Type Contract => inner.Contract;

        public IReadOnlyCollection<IGraphQlJoin> Joins => initial.Joins;

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new PostResolveComplexResolverBuilder(inner.ResolveComplex(serviceProvider, fieldContext), newResult =>
                new GraphQlDeferredScalarResult<TReturnType>(newResult, initial)
            );

        private Expression<Func<object, object?>> ResolveDeferredExpression => input => Execution.GraphQlResultExtensions.InvokeResult(inner, input).Data;
    }
}