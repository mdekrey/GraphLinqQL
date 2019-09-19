using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    class GraphQlExpressionResult<TReturnType> : IGraphQlResult<TReturnType>
    {
        public LambdaExpression UntypedResolver { get; }

        public LambdaExpression? Finalizer { get; }

        public IReadOnlyCollection<IGraphQlJoin> Joins { get; }

        public Type? Contract { get; }

        public GraphQlExpressionResult(LambdaExpression func, Type? contract = null, IReadOnlyCollection<IGraphQlJoin>? joins = null, LambdaExpression? finalizer = null)
        {
            this.UntypedResolver = func;
            this.Contract = contract;
            this.Joins = joins ?? ImmutableHashSet<IGraphQlJoin>.Empty;
            this.Finalizer = finalizer;
        }

        public static GraphQlExpressionResult<TReturnType> Construct<TInput>(Expression<Func<TInput, TReturnType>> func)
        {
            return new GraphQlExpressionResult<TReturnType>(func);
        }


        public IComplexResolverBuilder ResolveComplex(IServiceProvider serviceProvider)
        {
            if (Contract == null)
            {
                throw new InvalidOperationException("Result does not have a contract assigned to resolve complex objects");
            }

            var resolver = (IGraphQlResolvable)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, Contract);
            var accepts = resolver as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            var modelType = accepts.ModelType;
            accepts.Original = (IGraphQlResultFactory)Activator.CreateInstance(typeof(GraphQlResultFactory<>).MakeGenericType(modelType));

            return new ComplexResolverBuilder(
                resolver,
                ToResult,
                modelType
            );
        }
        
        private IGraphQlResult ToResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
        {
            var visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(this.UntypedResolver);
            var modelType = visitor.ModelType!;

            visitor.NewOperation = BuildJoinedSelector(resultSelector, joins, modelType);
            var returnResult = visitor.Visit(this.UntypedResolver.Body);

            //var returnResult = isList
            //        ? GetListReturnResult(modelType, mainBody)
            //    : IsSimple(this.UntypedResolver.Body)
            //        ? mainBody.Body.Replace(mainBody.Parameters[0], this.UntypedResolver.Body)
            //    : Expression.Invoke(Expression.Quote(mainBody), this.UntypedResolver.Body);

            if (this.Finalizer != null)
            {
                if (!this.Finalizer.Parameters[0].Type.IsAssignableFrom(returnResult.Type))
                {
                    throw new InvalidOperationException($"Unable to finalize - expected '{this.Finalizer.Parameters[0].Type.FullName}' but got '{returnResult.Type.FullName}'");
                }
                returnResult = this.Finalizer.Inline(returnResult);
            }

            var resultFunc = Expression.Lambda(returnResult, this.UntypedResolver.Parameters);
            return new GraphQlExpressionResult<object>(resultFunc, joins: this.Joins);
        }

        private Expression GetListReturnResult(Type modelType, LambdaExpression mainBody)
        {
            var getList = this.UntypedResolver.Body;
            if (!typeof(IQueryable<>).MakeGenericType(modelType).IsAssignableFrom(getList.Type))
            {
                var selected = Expression.Call(Resolve.asQueryable.MakeGenericMethod(mainBody.ReturnType), Expressions.CallEnumerableSelect(getList, mainBody));
                return Expressions.IfNotNull(this.UntypedResolver.Body, selected);
            }
            else
            {
                var selected = Expressions.CallQueryableSelect(getList, mainBody);
                return selected;
            }
        }

        private static LambdaExpression BuildJoinedSelector(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins, Type modelType)
        {
            var originalParameter = Expression.Parameter(modelType, "Original " + modelType.FullName);

            var mainBody = resultSelector.Inline(originalParameter)
                .Replace(joins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Inline(originalParameter)));
            var mainSelector = Expression.Lambda(mainBody, originalParameter);
            return mainSelector;
        }

        private static bool IsSimple(Expression body)
        {
            // TODO - we could probably make this nicer, but it doesn't seem to matter
            return body.NodeType == ExpressionType.Parameter || typeof(IQueryable).IsAssignableFrom(body.Type);
        }

        public IGraphQlResult As(Type contract)
        {
            var method = this.GetType().GetMethod(nameof(AsContract), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(contract);
            return (IGraphQlResult)method.Invoke(this, Array.Empty<object>());
        }

        public IGraphQlResult<TContract> As<TContract>() =>
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
            return new GraphQlExpressionResult<TContract>(newResolver, contract);
        }
    }

    static class GraphQlExpressionResult
    {
        public static IGraphQlResult Construct(Type returnType, LambdaExpression func, IReadOnlyCollection<IGraphQlJoin> joins)
        {
            var parameters = new object[] { func, joins };
            return (IGraphQlResult)typeof(GraphQlExpressionResult<>).MakeGenericType(returnType).GetConstructors().Single(c => c.GetParameters().Length == 2).Invoke(parameters);
        }
    }

    class GraphQlContractExpressionReplaceVisitor : ExpressionVisitor
    {
        public static readonly MethodInfo ContractPlaceholderMethod = typeof(GraphQlContractExpressionReplaceVisitor).GetMethod(nameof(ContractPlaceholder), BindingFlags.Static | BindingFlags.NonPublic);

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