using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
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