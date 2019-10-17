using System;
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
        public FieldContext FieldContext { get; }
        private readonly IReadOnlyList<Func<LambdaExpression, LambdaExpression>> postprocessBuild;

        public LambdaExpression Preamble { get; }
        public LambdaExpression Body { get; }

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        internal protected GraphQlExpressionScalarResult(
            FieldContext fieldContext,
            LambdaExpression preamble,
            LambdaExpression body,
            IReadOnlyList<Func<LambdaExpression, LambdaExpression>> postprocessBuild,
            IReadOnlyCollection<IGraphQlJoin> joins)
        {
            this.FieldContext = fieldContext;
            this.Preamble = preamble;
            this.Body = Expression.Lambda(body.Body.Box(), body.Parameters);
            this.postprocessBuild = postprocessBuild;
            this.Joins = joins;

            var visitor = new PreambleReplacement(Body);
            var result = (LambdaExpression)visitor.Replace(Preamble);
            // TODO - do some more type checking here
            //if (!typeof(TReturnType).IsAssignableFrom(body.Parameters[0].Type))
            //{
            //    throw new InvalidOperationException($"ScalarResult claimed to return '{typeof(TReturnType).FullName}' but is returning '{result.ReturnType.FullName}'");
            //}
        }

        public IGraphQlObjectResult<T> AsContract<T>(IContract contract, Func<Expression, Expression> bodyWrapper)
        {
            var newResolver = Expression.Lambda(bodyWrapper(Body.Body), Body.Parameters);
            return new GraphQlExpressionObjectResult<T>(new GraphQlExpressionScalarResult<object>(FieldContext, Preamble, newResolver, postprocessBuild, Joins), contract);
        }

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            AsContract<TContract>(SafeContract(typeof(TContract)), body => GraphQlContractExpression.ResolveContract(body, 0));

        private IContract SafeContract(Type contractType)
        {
            var currentReturnType = Body.Body.Unbox().Type;
            var acceptsInterface = contractType.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>))
                .Where(iface => iface.GetGenericArguments()[0].IsAssignableFrom(currentReturnType))
                .FirstOrDefault();
            if (acceptsInterface == null)
            {
                throw new InvalidOperationException($"Given contract {contractType.FullName} does not accept type {currentReturnType.FullName}");
            }
            return new ContractMapping(ContractMapping.GetTypeName(contractType), contractType, currentReturnType);
        }

        public LambdaExpression ConstructResult()
        {
            // Cases:
            // 1. Nothing special, Preamble return is inlined into Body's parameter
            // 2. Preamble has placeholder for "return" value that is passed to Body, which is inlined
            // 3. Preamble has quoted lambda for full Body lambda expression

            var initialBuild = new PreambleReplacement(Body).Replace(Preamble);
            var result = this.postprocessBuild.Aggregate(initialBuild, (current, postBuild) => postBuild(current));
            return result;
        }

        public IGraphQlScalarResult AddPostBuild(Func<LambdaExpression, LambdaExpression> postBuild)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(FieldContext, Preamble, Body, postprocessBuild.Concat(new[] { postBuild }).ToArray(), this.Joins);
        }

        public IGraphQlScalarResult<T> UpdatePreamble<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust)
        {
            return new GraphQlExpressionScalarResult<T>(FieldContext, preambleAdjust(Preamble), (Expression<Func<T, T>>)(_ => _), postprocessBuild, this.Joins);
        }

        public IGraphQlScalarResult<T> UpdateBody<T>(Func<LambdaExpression, LambdaExpression> bodyAdjust)
        {
            return new GraphQlExpressionScalarResult<T>(FieldContext, Preamble, bodyAdjust(Body), postprocessBuild, this.Joins);
        }

        public IGraphQlScalarResult<T> UpdatePreambleAndBody<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust, Func<LambdaExpression, LambdaExpression> bodyAdjust)
        {
            return new GraphQlExpressionScalarResult<T>(FieldContext, preambleAdjust(Preamble), bodyAdjust(Body), postprocessBuild, this.Joins);
        }

        internal static IGraphQlScalarResult<TReturnType> Constant(TReturnType result, FieldContext fieldContext)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(fieldContext, (Expression<Func<object?, TReturnType>>)(_ => result), (Expression<Func<TReturnType, TReturnType>>)(_ => _), EmptyArrayHelper.Empty< Func<LambdaExpression, LambdaExpression>>(), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

        internal static IGraphQlScalarResult<TReturnType> Simple(FieldContext fieldContext, LambdaExpression newFunc)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(fieldContext, newFunc, (Expression<Func<TReturnType, TReturnType>>)(_ => _), EmptyArrayHelper.Empty<Func<LambdaExpression, LambdaExpression>>(), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

        internal static IGraphQlScalarResult<TReturnType> CreateJoin(FieldContext fieldContext, LambdaExpression newFunc, IGraphQlJoin join)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(fieldContext, newFunc, (Expression<Func<TReturnType, TReturnType>>)(_ => _), EmptyArrayHelper.Empty<Func<LambdaExpression, LambdaExpression>>(), ImmutableHashSet.Create(join));
        }
    }

    class GraphQlExpressionObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly GraphQlContractExpressionReplaceVisitor visitor;

        public GraphQlExpressionObjectResult(
            IGraphQlScalarResult resolution,
            IContract contract)
        {
            this.Resolution = resolution;
            this.Contract = contract ?? throw new ArgumentException("Expected a contract but had none.", nameof(resolution));

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

        private IGraphQlScalarResult<object> ToResult(IReadOnlyList<LambdaExpression> joinedSelector)
        {
            visitor.NewOperations = joinedSelector;
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