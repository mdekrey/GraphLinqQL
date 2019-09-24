using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    internal static class EmptyObjectArrayContainer
    {
        public static readonly object[] Objects = new object[0];
    }

    class GraphQlExpressionResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        public IGraphQlParameterResolverFactory ParameterResolverFactory { get; }

        public LambdaExpression UntypedResolver { get; }

        public LambdaExpression? Finalizer { get; }

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        public Type? Contract { get; }

        public GraphQlExpressionResult(
            IGraphQlParameterResolverFactory parameterResolverFactory,
            LambdaExpression func,
            Type? contract = null,
            IReadOnlyCollection<IGraphQlJoin>? joins = null,
            LambdaExpression? finalizer = null)
        {
            this.ParameterResolverFactory = parameterResolverFactory;
            this.UntypedResolver = func;
            this.Contract = contract;
            this.Joins = joins ?? ImmutableHashSet<IGraphQlJoin>.Empty;
            this.Finalizer = finalizer;
        }

        public static GraphQlExpressionResult<TReturnType> Construct<TInput>(IGraphQlParameterResolverFactory parameterResolverFactory, Expression<Func<TInput, TReturnType>> func)
        {
            return new GraphQlExpressionResult<TReturnType>(parameterResolverFactory, func);
        }


        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider)
        {
            if (Contract == null)
            {
                throw new InvalidOperationException("Result does not have a contract assigned to resolve complex objects");
            }

            var resolver = serviceProvider.GetResolverContract(Contract);
            var accepts = resolver as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            var modelType = accepts.ModelType;
            accepts.Original = GraphQlResultFactory.Construct(modelType, serviceProvider);

            return new ComplexResolverBuilder(
                resolver,
                ToResult,
                modelType,
                ParameterResolverFactory
            );
        }
        
        private IGraphQlResult ToResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
        {
            var visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(this.UntypedResolver);
            var modelType = visitor.ModelType!;

            visitor.NewOperation = BuildJoinedSelector(resultSelector, joins, modelType);
            var returnResult = visitor.Visit(this.UntypedResolver.Body);

            if (this.Finalizer != null)
            {
                if (!this.Finalizer.Parameters[0].Type.IsAssignableFrom(returnResult.Type))
                {
                    throw new InvalidOperationException($"Unable to finalize - expected '{this.Finalizer.Parameters[0].Type.FullName}' but got '{returnResult.Type.FullName}'");
                }
                returnResult = this.Finalizer.Inline(returnResult);
            }

            var resultFunc = Expression.Lambda(returnResult, this.UntypedResolver.Parameters);
            return new GraphQlExpressionResult<object>(ParameterResolverFactory, resultFunc, joins: this.Joins);
        }

        private static LambdaExpression BuildJoinedSelector(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins, Type modelType)
        {
            var originalParameter = Expression.Parameter(modelType, "Original " + modelType.FullName);

            var mainBody = resultSelector.Inline(originalParameter)
                .Replace(joins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Inline(originalParameter)));
            var mainSelector = Expression.Lambda(mainBody, originalParameter);
            return mainSelector;
        }

        public IGraphQlResult As(Type contract)
        {
            var method = this.GetType().GetMethod(nameof(AsContract), BindingFlags.Instance | BindingFlags.NonPublic)!.MakeGenericMethod(contract);
            return (IGraphQlResult)method.Invoke(this, EmptyObjectArrayContainer.Objects)!;
        }

        public IGraphQlResult<TContract> As<TContract>() where TContract : IGraphQlAccepts<TReturnType> =>
            AsContract<TContract>();

        private IGraphQlResult<TContract> AsContract<TContract>()
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
                throw new ArgumentException($"Given contract {contract.FullName} does not accept type {currentReturnType.FullName}", nameof(contract));
            }
            var newResolver = Expression.Lambda(Expression.Call(GraphQlContractExpressionReplaceVisitor.ContractPlaceholderMethod, UntypedResolver.Body), UntypedResolver.Parameters);
            return new GraphQlExpressionResult<TContract>(ParameterResolverFactory, newResolver, contract);
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

        private static object? ContractPlaceholder(object input) => null;

        public LambdaExpression? NewOperation { get; set; }

        public Type? ModelType { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == ContractPlaceholderMethod)
            {
                ModelType = node.Arguments[0].Type;
                if (NewOperation != null)
                {
                    return Visit(Expressions.Inline(NewOperation, node.Arguments[0]));
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}