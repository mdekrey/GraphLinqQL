﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    /// <summary>
    /// <p>
    /// GraphQL Results are assembled with 2 phases: preamble and body. Typically, the preamble and the
    /// body are conjoined and flow from one into the next. That isn't always the case, however. We use 
    /// placeholders in the Expressions to handle the dynamic swapping. Placeholders allow us to nest the final lambda
    /// inside other expressions rather than needing to rely on the return result.
    /// </p>
    /// 
    /// <p>
    /// The preamble is not allowed to be modified after the link to the body is set. This allows us to
    /// have a known type outgoing from the preamble into the body. The goal of the preamble is to have
    /// simple enough Expressions that it can be handled by EF Core's SQL generation visitors, and allow us to move
    /// any portions that cannot be handled by EF Core to the body.
    /// </p>
    /// 
    /// <p>
    /// For the body placeholder, we use <see cref="GraphQlContractExpressionReplaceVisitor.ContractPlaceholderMethod" />
    /// when returning a GraphQL Object. This returns a C# object, which is appropriate after complex resolution.
    /// Scalar results do not use this placeholder. The return result of the body is the returned result of the
    /// GraphQL Result.
    /// </p>
    /// 
    /// <p>
    /// Joins are provided to a parent Object Result in order to share expressions between properties. This is intended
    /// to share information for before the preamble itself so that EF Core can produce a better overall query.
    /// </p>
    /// </summary>
    class GraphQlExpressionScalarResult<TReturnType> : IGraphQlScalarResult<TReturnType>
    {
        public LambdaExpression Preamble { get; }
        public LambdaExpression Body { get; }

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        protected GraphQlExpressionScalarResult(
            LambdaExpression preamble,
            LambdaExpression body,
            IReadOnlyCollection<IGraphQlJoin> joins)
        {
            this.Preamble = preamble;
            this.Body = body;
            this.Joins = joins;

            var visitor = new GraphQlPreambleExpressionReplaceVisitor(Body);
            var result = (LambdaExpression)visitor.Visit(Preamble);
            if (!typeof(TReturnType).IsAssignableFrom(body.Parameters[0].Type))
            {
                visitor.Visit(Preamble);
                throw new InvalidOperationException($"ScalarResult claimed to return '{typeof(TReturnType).FullName}' but is returning '{result.ReturnType.FullName}'");
            }
        }

        public IGraphQlObjectResult<T> AsContract<T>(IContract contract)
        {
            var newResolver = Expression.Lambda(Expression.Call(GraphQlContractExpressionReplaceVisitor.ContractPlaceholderMethod, Body.Body), Body.Parameters);
            return new GraphQlExpressionObjectResult<T>(new GraphQlExpressionScalarResult<object>(Preamble, newResolver, Joins), contract);
        }

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            AsContract<TContract>(SafeContract(typeof(TContract)));

        private IContract SafeContract(Type contractType)
        {
            var currentReturnType = Body.ReturnType;
            var acceptsInterface = contractType.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>))
                .Where(iface => iface.GetGenericArguments()[0].IsAssignableFrom(currentReturnType))
                .FirstOrDefault();
            if (acceptsInterface == null)
            {
                throw new InvalidOperationException($"Given contract {contractType.FullName} does not accept type {currentReturnType.FullName}");
            }
            return new ContractMapping(contractType);
        }

        public LambdaExpression ConstructResult()
        {
            // Cases:
            // 1. Nothing special, Preamble return is inlined into Body's parameter
            // 2. Preamble has placeholder for "return" value that is passed to Body, which is inlined
            // 3. Preamble has quoted lambda for full Body lambda expression

            var visitor = new GraphQlPreambleExpressionReplaceVisitor(Body);
            var result = (LambdaExpression)visitor.Visit(Preamble);
            return visitor.Exchanged
                ? result
                : Expression.Lambda(Body.Inline(Preamble.Body), Preamble.Parameters);
        }

        public IGraphQlScalarResult<T> UpdatePreamble<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust)
        {
            return new GraphQlExpressionScalarResult<T>(preambleAdjust(Preamble), (Expression<Func<T, T>>)(_ => _), this.Joins);
        }

        public IGraphQlScalarResult<T> UpdateBody<T>(Func<LambdaExpression, LambdaExpression> bodyAdjust)
        {
            return new GraphQlExpressionScalarResult<T>(Preamble, bodyAdjust(Body), this.Joins);
        }

        public IGraphQlScalarResult<T> UpdatePreambleAndBody<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust, Func<LambdaExpression, LambdaExpression> bodyAdjust)
        {
            return new GraphQlExpressionScalarResult<T>(preambleAdjust(Preamble), bodyAdjust(Body), this.Joins);
        }

        internal static IGraphQlScalarResult<TReturnType> Constant(TReturnType result)
        {
            return new GraphQlExpressionScalarResult<TReturnType>((Expression<Func<object?, TReturnType>>)(_ => result), (Expression<Func<TReturnType, TReturnType>>)(_ => _), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

        internal static IGraphQlScalarResult<TReturnType> Simple(LambdaExpression newFunc)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(newFunc, (Expression<Func<TReturnType, TReturnType>>)(_ => _), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

        internal static IGraphQlScalarResult<TReturnType> CreateJoin(LambdaExpression newFunc, IGraphQlJoin join)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(newFunc, (Expression<Func<TReturnType, TReturnType>>)(_ => _), ImmutableHashSet.Create(join));
        }
    }

    class GraphQlExpressionObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly GraphQlContractExpressionReplaceVisitor visitor;

        public GraphQlExpressionObjectResult(
            IGraphQlScalarResult resolution,
            IContract contract)
        {
            if (contract == null)
            {
                throw new ArgumentException("Expected a contract but had none.", nameof(resolution));
            }
            this.Resolution = resolution;
            this.Contract = contract;

            visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(resolution.Body);
            if (visitor.ModelType == null)
            {
                throw new ArgumentException("The provided resolver did not have a contract.", nameof(resolution));
            }
        }

        public IContract Contract { get; }

        public IGraphQlScalarResult Resolution { get; }

        public IGraphQlObjectResult<T> AdjustResolution<T>(Func<IGraphQlScalarResult, IGraphQlScalarResult> p)
        {
            return new GraphQlExpressionObjectResult<T>(p(Resolution), Contract);
        }

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext)
        {
            return new ComplexResolverBuilder(
                Contract!,
                serviceProvider,
                ToResult,
                visitor.ModelType!,
                fieldContext
            );
        }

        private IGraphQlScalarResult ToResult(LambdaExpression joinedSelector)
        {
            visitor.NewOperation = joinedSelector;
            return Resolution.UpdateBody<object>(body =>
            {
                var returnResult = visitor.Visit(body.Body);
                return Expression.Lambda(returnResult, body.Parameters);
            });
        }

    }

    static class GraphQlExpressionResult
    {
        public static IGraphQlScalarResult Construct(Type returnType, LambdaExpression func, IReadOnlyCollection<IGraphQlJoin> joins)
        {
            return (IGraphQlScalarResult)Activator.CreateInstance(typeof(GraphQlExpressionScalarResult<>).MakeGenericType(returnType), func, joins)!;
        }
    }
}