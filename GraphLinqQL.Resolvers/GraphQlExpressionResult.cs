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
        public LambdaExpression UntypedResolver { get; }

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        public GraphQlExpressionScalarResult(
            LambdaExpression untypedResolver,
            IReadOnlyCollection<IGraphQlJoin> joins)
        {
            this.UntypedResolver = untypedResolver;
            this.Joins = joins;

            var visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(this.UntypedResolver);
            if (visitor.ModelType != null)
            {
                throw new ArgumentException("Was given a model but intended for a scalar.", nameof(untypedResolver));
            }
        }

        public IGraphQlObjectResult AsContract(Type contract)
        {
            var method = this.GetType().GetMethod(nameof(UnsafeAsContract), BindingFlags.Instance | BindingFlags.NonPublic)!.MakeGenericMethod(contract);
            return (IGraphQlObjectResult)method.Invoke(this, EmptyArrayHelper.Empty<object>())!;
        }

        public IGraphQlObjectResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            UnsafeAsContract<TContract>();

        private IGraphQlObjectResult<TContract> UnsafeAsContract<TContract>()
        {
            var contract = typeof(TContract);
            var currentReturnType = UntypedResolver.ReturnType;
            var acceptsInterface = contract.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>))
                .Where(iface => iface.GetGenericArguments()[0].IsAssignableFrom(currentReturnType))
                .FirstOrDefault();
            if (acceptsInterface == null)
            {
                throw new InvalidOperationException($"Given contract {contract.FullName} does not accept type {currentReturnType.FullName}");
            }
            var newResolver = Expression.Lambda(Expression.Call(GraphQlContractExpressionReplaceVisitor.ContractPlaceholderMethod, UntypedResolver.Body), UntypedResolver.Parameters);
            return new GraphQlExpressionObjectResult<TContract>(newResolver, contract, this.Joins);
        }
    }

    class GraphQlExpressionObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly GraphQlContractExpressionReplaceVisitor visitor;

        public GraphQlExpressionObjectResult(
            LambdaExpression untypedResolver,
            Type contract,
            IReadOnlyCollection<IGraphQlJoin> joins)
        {
            this.UntypedResolver = untypedResolver;
            this.Contract = contract;
            this.Joins = joins;

            visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(this.UntypedResolver);
            if (visitor.ModelType == null && contract != null)
            {
                throw new ArgumentException("The provided resolver did not have a contract.", nameof(untypedResolver));
            }
            else if (contract == null && visitor.ModelType != null)
            {
                throw new ArgumentException("Expected a contract but had none.", nameof(untypedResolver));
            }
        }

        public Type Contract { get; }

        public LambdaExpression UntypedResolver { get; }

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext)
        {
            return new ComplexResolverBuilder(
                Contract!,
                serviceProvider,
                ToResult,
                visitor.ModelType!
            );
        }

        private IGraphQlScalarResult ToResult(LambdaExpression joinedSelector)
        {
            visitor.NewOperation = joinedSelector;
            var returnResult = visitor.Visit(this.UntypedResolver.Body);
            var resultFunc = Expression.Lambda(returnResult, this.UntypedResolver.Parameters);
            return new GraphQlExpressionScalarResult<object>(resultFunc, joins: this.Joins);
        }

    }

    static class GraphQlExpressionResult
    {
        public static IGraphQlScalarResult Construct(Type returnType, LambdaExpression func, IReadOnlyCollection<IGraphQlJoin> joins)
        {
            return (IGraphQlScalarResult)Activator.CreateInstance(typeof(GraphQlExpressionScalarResult<>).MakeGenericType(returnType), func, joins)!;
        }
    }

    class GraphQlContractExpressionReplaceVisitor : ExpressionVisitor
    {
        public static readonly MethodInfo ContractPlaceholderMethod = typeof(GraphQlContractExpressionReplaceVisitor).GetMethod(nameof(ContractPlaceholder), BindingFlags.Static | BindingFlags.NonPublic)!;

#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
        private static object? ContractPlaceholder(object input) => null;
#pragma warning restore CA1801 // Remove unused parameter

        public LambdaExpression? NewOperation { get; set; }

        public Type? ModelType { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == ContractPlaceholderMethod)
            {
                ModelType = node.Arguments[0].Type;
                if (NewOperation != null)
                {
                    return Visit(NewOperation.Inline(node.Arguments[0]));
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}