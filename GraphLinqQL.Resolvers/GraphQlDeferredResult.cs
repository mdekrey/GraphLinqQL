using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    class GraphQlDeferredScalarResult<TReturnType> : IGraphQlScalarResult<TReturnType>
    {
        private readonly IGraphQlScalarResult inner;
        private readonly IGraphQlScalarResult preamble;

        public GraphQlDeferredScalarResult(
            IGraphQlScalarResult inner,
            IGraphQlScalarResult preamble)
        {
            this.inner = inner;
            this.preamble = preamble;
        }

        public LambdaExpression UntypedResolver =>
            Expression.Lambda(ResolveDeferredExpression.Inline(preamble.UntypedResolver.Body), preamble.UntypedResolver.Parameters);

        public IReadOnlyCollection<IGraphQlJoin> Joins => preamble.Joins;

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            new GraphQlDeferredObjectResult<TContract>(inner.AsContract(typeof(TContract)), preamble);

        public IGraphQlObjectResult AsContract(Type contract) =>
            (IGraphQlObjectResult)Activator.CreateInstance(typeof(GraphQlDeferredObjectResult<>).MakeGenericType(contract), new object[] { inner.AsContract(contract), preamble });

        private Expression<Func<object, object?>> ResolveDeferredExpression => input => Execution.GraphQlResultExtensions.InvokeResult(inner, input).Data;
    }

    class GraphQlDeferredObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly IGraphQlObjectResult inner;
        private readonly IGraphQlScalarResult preamble;

        public GraphQlDeferredObjectResult(
            IGraphQlObjectResult inner,
            IGraphQlScalarResult preamble)
        {
            this.inner = inner;
            this.preamble = preamble;
        }

        public LambdaExpression UntypedResolver => throw new NotSupportedException();
            //Expression.Lambda(ResolveDeferredExpression.Inline(preamble.UntypedResolver.Body), preamble.UntypedResolver.Parameters);

        public Type Contract => inner.Contract;

        public IReadOnlyCollection<IGraphQlJoin> Joins => preamble.Joins;

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new PostResolveComplexResolverBuilder(inner.ResolveComplex(serviceProvider, fieldContext), newResult =>
                new GraphQlDeferredScalarResult<TReturnType>(newResult, preamble)
            );

        //private Expression<Func<object, object?>> ResolveDeferredExpression => input => Execution.GraphQlResultExtensions.InvokeResult(inner, input).Data;
    }
}