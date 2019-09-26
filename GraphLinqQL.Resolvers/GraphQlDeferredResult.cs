using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    class GraphQlDeferredResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        public IGraphQlParameterResolverFactory ParameterResolverFactory { get; }

        public LambdaExpression UntypedResolver { get; }

        public LambdaExpression? Finalizer => innerResult.Finalizer;

        private readonly IGraphQlResult<TReturnType> innerResult;

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        public Type? Contract => innerResult.Contract;

        public GraphQlDeferredResult(
            IGraphQlParameterResolverFactory parameterResolverFactory,
            LambdaExpression outerResolver,
            IGraphQlResult<TReturnType> innerResult,
            IReadOnlyCollection<IGraphQlJoin>? joins = null)
        {
            this.ParameterResolverFactory = parameterResolverFactory;
            this.UntypedResolver = outerResolver;
            this.innerResult = innerResult;
            this.Joins = joins ?? ImmutableHashSet<IGraphQlJoin>.Empty;
        }

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider)
        {
            if (Contract == null)
            {
                throw new InvalidOperationException("Result does not have a contract assigned to resolve complex objects");
            }

            throw new NotImplementedException();
        }

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType>
        {
            throw new NotImplementedException();
        }

        public IGraphQlResult AsContract(Type contract)
        {
            throw new NotImplementedException();
        }
    }
}