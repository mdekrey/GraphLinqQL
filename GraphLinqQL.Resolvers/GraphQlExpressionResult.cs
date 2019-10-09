using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
#if NETFRAMEWORK
    internal static class EmptyObjectArrayContainer
    {
        public static readonly object[] Objects = new object[0];
    }
#endif

    class GraphQlExpressionResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        public LambdaExpression UntypedResolver { get; }

        private readonly GraphQlContractExpressionReplaceVisitor visitor;

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        public Type? Contract { get; }
        
        public bool ShouldSubselect => Contract != null;

        public GraphQlExpressionResult(
            LambdaExpression untypedResolver,
            Type? contract = null,
            IReadOnlyCollection<IGraphQlJoin>? joins = null)
        {
            this.UntypedResolver = untypedResolver;
            this.Contract = contract;
            this.Joins = joins ?? ImmutableHashSet<IGraphQlJoin>.Empty;

            visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(this.UntypedResolver);
            if (visitor.ModelType == null && contract != null)
            {
                throw new ArgumentException("The provided resolver did not have a contract.", nameof(untypedResolver));
            } else if (contract == null && visitor.ModelType != null)
            {
                throw new ArgumentException("Expected a contract but had none.", nameof(untypedResolver));
            }
        }

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider, FieldContext fieldContext)
        {
            if (Contract == null)
            {
                // FIXME: Maybe somehow get the creating contract info here, knowing that some reuslts are created so don't have a root?
                throw new InvalidOperationException("Result does not have a contract assigned to resolve complex objects").AddGraphQlError(WellKnownErrorCodes.NoSubselectionAllowed, fieldContext.Locations, new { fieldName = fieldContext.Name, type = "(May be root type)" });
            }

            return new ComplexResolverBuilder(
                Contract!,
                serviceProvider,
                ToResult,
                visitor.ModelType!
            );
        }

        private IGraphQlResult ToResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
        {
            var modelType = visitor.ModelType!;

            visitor.NewOperation = BuildJoinedSelector(resultSelector, joins, modelType);
            var returnResult = visitor.Visit(this.UntypedResolver.Body);

            var resultFunc = Expression.Lambda(returnResult, this.UntypedResolver.Parameters);
            return new GraphQlExpressionResult<object>(resultFunc, joins: this.Joins);
        }

        private static LambdaExpression BuildJoinedSelector(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins, Type modelType)
        {
            var originalParameter = Expression.Parameter(modelType, "Original " + modelType.FullName);

            var mainBody = resultSelector.Inline(originalParameter)
                .Replace(joins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Inline(originalParameter)));
            var mainSelector = Expression.Lambda(mainBody, originalParameter);
            return mainSelector;
        }

        public IGraphQlResult AsContract(Type contract)
        {
            var method = this.GetType().GetMethod(nameof(UnsafeAsContract), BindingFlags.Instance | BindingFlags.NonPublic)!.MakeGenericMethod(contract);
#if NETFRAMEWORK
            return (IGraphQlResult)method.Invoke(this, EmptyObjectArrayContainer.Objects)!;
#else
            return (IGraphQlResult)method.Invoke(this, Array.Empty<object>())!;
#endif
        }

        public IGraphQlResult<TContract> AsContract<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            UnsafeAsContract<TContract>();

        private IGraphQlResult<TContract> UnsafeAsContract<TContract>()
        {
            if (Contract != null)
            {
                throw new InvalidOperationException($"Can't put a contract on top of a contract; already was assigned {Contract.FullName}");
            }
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
            return new GraphQlExpressionResult<TContract>(newResolver, contract);
        }
    }

    static class GraphQlExpressionResult
    {
        public static IGraphQlResult Construct(Type returnType, LambdaExpression func, IReadOnlyCollection<IGraphQlJoin> joins)
        {
            return (IGraphQlResult)Activator.CreateInstance(typeof(GraphQlExpressionResult<>).MakeGenericType(returnType), func, joins)!;
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