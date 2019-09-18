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

        public GraphQlExpressionResult(LambdaExpression func)
            : this(func, ImmutableHashSet<IGraphQlJoin>.Empty) { }

        public GraphQlExpressionResult(LambdaExpression func, IReadOnlyCollection<IGraphQlJoin> joins)
            : this(func, joins, null)
        { }

        public GraphQlExpressionResult(LambdaExpression func, IReadOnlyCollection<IGraphQlJoin> joins, LambdaExpression? finalizer)
        {
            this.UntypedResolver = func;
            this.Joins = joins;
            this.Finalizer = finalizer;
        }

        public static GraphQlExpressionResult<TReturnType> Construct<TInput>(Expression<Func<TInput, TReturnType>> func)
        {
            return new GraphQlExpressionResult<TReturnType>(func);
        }
        public Type ResultType => UntypedResolver.ReturnType;



        public IComplexResolverBuilder ResolveComplex(IServiceProvider serviceProvider)
        {
            var actualContractType = TypeSystem.GetElementType(typeof(TReturnType));
            var actualModelType = this.UntypedResolver.ReturnType;

            var resolver = (IGraphQlResolvable)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, actualContractType);
            var accepts = resolver as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            var modelType = accepts.ModelType;
            if (!modelType.IsAssignableFrom(actualModelType) && !modelType.IsAssignableFrom(TypeSystem.GetElementType(actualModelType)))
            {
                throw new ArgumentException("Contract not valid for incoming model");
            }
            accepts.Original = (IGraphQlResultFactory)Activator.CreateInstance(typeof(GraphQlResultFactory<>).MakeGenericType(modelType));

            return new ComplexResolverBuilder(
                resolver,
                ToResult,
                modelType
            );
        }
        
        private IGraphQlResult ToResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
        {
            //resultSelector.Parameters[0].Type == target.UntypedResolver.ReturnType
            var isList = !resultSelector.Parameters[0].Type.IsAssignableFrom(this.UntypedResolver.ReturnType)
                && resultSelector.Parameters[0].Type == TypeSystem.GetElementType(this.UntypedResolver.ReturnType);
            var modelType = isList ? TypeSystem.GetElementType(this.UntypedResolver.ReturnType)
                : this.UntypedResolver.ReturnType;
            var mainBody = BuildJoinedSelector(resultSelector, joins, isList ? TypeSystem.GetElementType(modelType) : modelType);

            var returnResult = isList
                    ? GetListReturnResult(modelType, mainBody)
                : IsSimple(this.UntypedResolver.Body)
                    ? mainBody.Body.Replace(mainBody.Parameters[0], this.UntypedResolver.Body)
                : Expression.Invoke(Expression.Quote(mainBody), this.UntypedResolver.Body);

            if (this.Finalizer != null)
            {
                if (!this.Finalizer.Parameters[0].Type.IsAssignableFrom(returnResult.Type))
                {
                    throw new InvalidOperationException($"Unable to finalize - expected '{this.Finalizer.Parameters[0].Type.FullName}' but got '{returnResult.Type.FullName}'");
                }
                returnResult = this.Finalizer.Body.Replace(this.Finalizer.Parameters[0], returnResult);
            }

            var resultFunc = Expression.Lambda(returnResult, this.UntypedResolver.Parameters);
            return new GraphQlExpressionResult<object>(resultFunc, this.Joins);
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

            var mainBody = resultSelector.Body.Replace(resultSelector.Parameters[0], originalParameter)
                .Replace(joins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Body.Replace(join.Conversion.Parameters[0], originalParameter)));
            var mainSelector = Expression.Lambda(mainBody, originalParameter);
            return mainSelector;
        }

        private static bool IsSimple(Expression body)
        {
            // TODO - we could probably make this nicer, but it doesn't seem to matter
            return body.NodeType == ExpressionType.Parameter || typeof(IQueryable).IsAssignableFrom(body.Type);
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
}