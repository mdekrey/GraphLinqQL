﻿using System;
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

        public bool ShouldSubselect => false;

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
        private readonly IGraphQlScalarResult outer;

        public GraphQlDeferredObjectResult(
            IGraphQlObjectResult inner,
            IGraphQlScalarResult outer)
        {
            this.inner = inner;
            this.outer = outer;
        }

        public LambdaExpression UntypedResolver =>
            Expression.Lambda(ResolveDeferredExpression.Inline(outer.UntypedResolver.Body), outer.UntypedResolver.Parameters);

        public Type Contract => inner.Contract;

        public bool ShouldSubselect => true;

        public IReadOnlyCollection<IGraphQlJoin> Joins => outer.Joins;

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext) =>
            new PostResolveComplexResolverBuilder(inner.ResolveComplex(serviceProvider, fieldContext), newResult =>
                new GraphQlDeferredScalarResult<TReturnType>(newResult, outer)
            );

        private Expression<Func<object, object?>> ResolveDeferredExpression => input => Execution.GraphQlResultExtensions.InvokeResult(inner, input).Data;
    }
}