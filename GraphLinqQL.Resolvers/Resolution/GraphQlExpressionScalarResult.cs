using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    /// <summary>
    /// <p>
    /// GraphQL Results are assembled in layers of lambdas. Each lambda either returns a new value which
    /// is used as the input for the next, or has one of a few "Preamble" placeholders to either inline the
    /// following functionality (such as in a .ContinueWith for a task) or receive a callback for the
    /// remaining functionality.
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
        private readonly IReadOnlyList<LambdaExpression> resolvers;
        private readonly IReadOnlyList<ExpressionVisitor> constructionVisitors;

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        internal protected GraphQlExpressionScalarResult(
            FieldContext fieldContext,
            IReadOnlyList<LambdaExpression> resolvers,
            IReadOnlyList<ExpressionVisitor> constructionVisitors,
            IReadOnlyCollection<IGraphQlJoin> joins)
        {
            this.FieldContext = fieldContext;
            this.resolvers = resolvers;
            this.constructionVisitors = constructionVisitors;
            this.Joins = joins;
        }

        private static LambdaExpression Identity(Type type)
        {
            var param = Expression.Parameter(type, "identityOf" + type.Name);
            return Expression.Lambda(param, param);
        }

        public IGraphQlObjectResult<T> AsContract<T>(IContract contract, LambdaExpression bodyWrapper)
        {
            return new GraphQlExpressionObjectResult<T>(this.AddResolve<object>(bodyWrapper), contract);
        }

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            AsContract<TContract>(SafeContract(typeof(TContract)), GraphQlContractExpression.ResolveContractIndexed(0));

        private IContract SafeContract(Type contractType)
        {
            var currentReturnType = GetCurrentReturnType();
            var acceptsInterface = contractType.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>))
                .Where(iface => iface.GetGenericArguments()[0].IsAssignableFrom(currentReturnType))
                .FirstOrDefault();
            if (acceptsInterface == null)
            {
                throw new InvalidOperationException($"Given contract {contractType.FullName} does not accept type {currentReturnType.FullName}");
            }
            return new ContractMapping(contractType, currentReturnType);
        }

        private Type GetCurrentReturnType()
        {
            return PreambleReplacement.GetExpectedReplacementType(resolvers[resolvers.Count - 1]);
        }

        public LambdaExpression ConstructResult()
        {
            var composed = resolvers.Reverse().Aggregate(
                Identity(GetCurrentReturnType()),
                (current, prev) => new PreambleReplacement(current).Replace(prev)
            );
            var finalResult = constructionVisitors.Aggregate(composed, (prev, next) => next.VisitAndConvert(prev, nameof(ConstructResult)));
            return finalResult;
        }

        public IGraphQlScalarResult<T> AddResolve<T>(Func<ParameterExpression, LambdaExpression> resolve)
        {
            var returnType = GetCurrentReturnType();
            var param = Expression.Parameter(returnType, "resolve" + returnType.Name);
            var next = resolve(param);
            if (next.Parameters[0].Type != param.Type)
            {
                // some coersion needed...
                if (!next.Parameters[0].Type.IsAssignableFrom(param.Type))
                {
                    throw new ArgumentException($"Unable to convert current resolved type of {param.Type.FullName} to {next.Parameters[0].Type.FullName}");
                }

                return new GraphQlExpressionScalarResult<T>(FieldContext, resolvers.Concat(new[] { Expression.Lambda(Expression.Convert(param, next.Parameters[0].Type), param), next }).ToArray(), constructionVisitors, Joins);
            }
            else
            {
                return new GraphQlExpressionScalarResult<T>(FieldContext, resolvers.Concat(new[] { next }).ToArray(), constructionVisitors, Joins);
            }
        }

        public IGraphQlScalarResult<T> ApplyVisitor<T>(ExpressionVisitor visitor)
        {
            return new GraphQlExpressionScalarResult<T>(FieldContext, resolvers.Select(v => visitor.VisitAndConvert(v, nameof(ApplyVisitor))).ToArray(), constructionVisitors, Joins);
        }

        public IGraphQlScalarResult<T> AddConstructionVisitor<T>(ExpressionVisitor visitor)
        {
            return new GraphQlExpressionScalarResult<T>(FieldContext, resolvers, constructionVisitors.Concat(new[] { visitor }).ToArray(), Joins);
        }

        internal static IGraphQlScalarResult<TReturnType> Constant(TReturnType result, FieldContext fieldContext)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(fieldContext, new[] { (Expression<Func<object?, TReturnType>>)(_ => result) }, EmptyArrayHelper.Empty<ExpressionVisitor>(), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

        internal static IGraphQlScalarResult<TReturnType> Simple(FieldContext fieldContext, LambdaExpression newFunc)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(fieldContext, new[] { newFunc }, EmptyArrayHelper.Empty<ExpressionVisitor>(), ImmutableHashSet<IGraphQlJoin>.Empty);
        }

        internal static IGraphQlScalarResult<TReturnType> CreateJoin(FieldContext fieldContext, LambdaExpression newFunc, IGraphQlJoin join)
        {
            return new GraphQlExpressionScalarResult<TReturnType>(fieldContext, new[] { newFunc }, EmptyArrayHelper.Empty<ExpressionVisitor>(), ImmutableHashSet.Create(join));
        }
    }
}