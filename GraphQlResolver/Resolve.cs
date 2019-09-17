using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    public static class Resolve
    {
        private static MethodInfo asQueryable = typeof(Queryable).GetMethods()
            .Where(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .Single();

        private static readonly IReadOnlyList<MethodInfo> complexResolvers = (from method in typeof(Resolve).GetMethods()
                                                                              where method.Name == nameof(ResolveComplex) && method.IsGenericMethodDefinition
                                                                              select method).ToArray();

        public static object GraphQlRoot(this IServiceProvider serviceProvider, Type t, Func<IComplexResolverBuilder, IGraphQlResult> resolver)
        {
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(serviceProvider);
            var resolved = resolver(resultFactory.Resolve(a => a).Convertable().As(t).ResolveComplex());
            var expression = resolved?.ResolveExpression<GraphQlRoot>()!;
            expression = expression.CollapseDoubleSelect();
            var queryable = Enumerable.Repeat(new GraphQlRoot(), 1).AsQueryable().Select(expression);
            return queryable.Single();
        }

        public static object GraphQlRoot<T>(this IServiceProvider serviceProvider, Func<IComplexResolverBuilder<T>, IGraphQlResult> resolver)
            where T : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        {
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(serviceProvider);
            var resolved = resolver(resultFactory.Resolve(a => a).Convertable().As<T>().ResolveComplex());
            var expression = resolved?.ResolveExpression<GraphQlRoot>();
            var queryable = Enumerable.Repeat(new GraphQlRoot(), 1).AsQueryable().Select(expression);
            return queryable.Single();
        }

        public static IComplexResolverBuilder ResolveComplex(this IGraphQlResult target)
        {
            var paramType = target.GetType();
            var resultType = paramType.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IGraphQlResult<>)).Select(iface => iface.GetGenericArguments()[0]).First();
            var contractType = TypeSystem.GetElementType(resultType) ?? resultType;

            if (typeof(IGraphQlResolvable).IsAssignableFrom(contractType))
            {
                var method = complexResolvers.Select(m => m.MakeGenericMethod(contractType)).First(m => m.GetParameters().Single().ParameterType.IsAssignableFrom(paramType));
                return (IComplexResolverBuilder)method.Invoke(null, new[] { target });
            }
            throw new ArgumentException("Not a resolvable complex type", nameof(target));
        }

        public static IComplexResolverBuilder<TContract> ResolveComplex<TContract>(this IGraphQlResult<TContract> target)
            where TContract : IGraphQlResolvable
        {
            var actualContractType = TypeSystem.GetElementType(target.GetType().GetGenericArguments()[0]);
            
            GetContract<TContract>(target, actualContractType, out var resolver, out var modelType);

            return new ComplexResolverBuilder<TContract>(
                resolver,
                GetResultConverter(target),
                modelType
            );

        }

        public static IComplexResolverBuilder<TContract> ResolveComplex<TContract>(this IGraphQlResult<IEnumerable<TContract>> target)
            where TContract : IGraphQlResolvable
        {
            if (target is IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult)
            {
                return new UnionResolverBuilder<TContract>(unionResult);
            }
            var actualContractType = TypeSystem.GetElementType(target.GetType().GetGenericArguments()[0]);
            GetContract<TContract>(target, actualContractType, out var resolver, out var modelType);

            return new ComplexResolverBuilder<TContract>(
                resolver,
                GetResultConverter(target),
                modelType
            );
        }

        private static Func<LambdaExpression, ImmutableHashSet<IGraphQlJoin>, IGraphQlResult> GetResultConverter(IGraphQlResult target)
        {
            IGraphQlResult ToResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
            {
                //resultSelector.Parameters[0].Type == target.UntypedResolver.ReturnType
                var isList = !resultSelector.Parameters[0].Type.IsAssignableFrom(target.UntypedResolver.ReturnType)
                    && resultSelector.Parameters[0].Type == TypeSystem.GetElementType(target.UntypedResolver.ReturnType);
                var modelType = isList ? TypeSystem.GetElementType(target.UntypedResolver.ReturnType)
                    : target.UntypedResolver.ReturnType;
                var mainBody = BuildJoinedSelector(resultSelector, joins, isList ? TypeSystem.GetElementType(modelType) : modelType);

                var returnResult = isList
                        ? GetListReturnResult(modelType, mainBody)
                    : IsSimple(target.UntypedResolver.Body)
                        ? mainBody.Body.Replace(mainBody.Parameters[0], target.UntypedResolver.Body)
                    : Expression.Invoke(Expression.Quote(mainBody), target.UntypedResolver.Body);

                if (target.Finalizer != null)
                {
                    if (!target.Finalizer.Parameters[0].Type.IsAssignableFrom(returnResult.Type))
                    {
                        throw new InvalidOperationException($"Unable to finalize - expected '{target.Finalizer.Parameters[0].Type.FullName}' but got '{returnResult.Type.FullName}'");
                    }
                    returnResult = target.Finalizer.Body.Replace(target.Finalizer.Parameters[0], returnResult);
                }

                var resultFunc = Expression.Lambda(returnResult, target.UntypedResolver.Parameters);
                return new GraphQlExpressionResult<object>(resultFunc, target.ServiceProvider, target.Joins);
            }

            Expression GetListReturnResult(Type modelType, LambdaExpression mainBody)
            {
                var getList = target.UntypedResolver.Body;
                if (!typeof(IQueryable<>).MakeGenericType(modelType).IsAssignableFrom(getList.Type))
                {
                    var selected = Expression.Call(Resolve.asQueryable.MakeGenericMethod(mainBody.ReturnType), Expressions.CallEnumerableSelect(getList, mainBody));
                    return Expressions.IfNotNull(target.UntypedResolver.Body, selected);
                }
                else
                {
                    var selected = Expressions.CallQueryableSelect(getList, mainBody);
                    return selected;
                }
            }

            return ToResult;
        }

        private static bool IsSimple(Expression body)
        {
            // TODO - we could probably make this nicer, but it doesn't seem to matter
            return body.NodeType == ExpressionType.Parameter || typeof(IQueryable).IsAssignableFrom(body.Type);
        }

        private static LambdaExpression BuildJoinedSelector(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins, Type modelType)
        {
            var originalParameter = Expression.Parameter(modelType, "Original " + modelType.FullName);

            var mainBody = resultSelector.Body.Replace(resultSelector.Parameters[0], originalParameter)
                .Replace(joins.ToDictionary(join => join.Placeholder as Expression, join => join.Conversion.Body.Replace(join.Conversion.Parameters[0], originalParameter)));
            var mainSelector = Expression.Lambda(mainBody, originalParameter);
            return mainSelector;
        }

        private static void GetContract<TContract>(IGraphQlResult target, Type actualContractType, out TContract resolver, out Type modelType) where TContract : IGraphQlResolvable
        {
            var actualModelType = target.UntypedResolver.ReturnType;

            resolver = (TContract)ActivatorUtilities.GetServiceOrCreateInstance(target.ServiceProvider, actualContractType);
            var accepts = resolver as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            modelType = accepts.ModelType;
            if (!modelType.IsAssignableFrom(actualModelType) && !modelType.IsAssignableFrom(TypeSystem.GetElementType(actualModelType)))
            {
                throw new ArgumentException("Contract not valid for incoming model");
            }
            accepts.Original = (IGraphQlResultFactory)Activator.CreateInstance(typeof(GraphQlResultFactory<>).MakeGenericType(modelType), target.ServiceProvider);
        }

        public static Expression<Func<TInput, object>> ResolveExpression<TInput>(this IGraphQlResult result)
        {
            return result.UntypedResolver.CastAndBoxSingleInput<TInput>();
        }

        public static ConvertableResult<TModel> ConvertableValue<TModel>(this IGraphQlResult<TModel> target)
            where TModel : struct
        {
            return new ConvertableResult<TModel>(target);
        }

        public static ConvertableListResult<TModel> ConvertableValueList<TModel>(this IGraphQlResult<IEnumerable<TModel>?> target)
            where TModel : struct
        {
            return new ConvertableListResult<TModel>(target!);
        }

        public static ConvertableResult<TModel> Convertable<TModel>(this IGraphQlResult<TModel?> target)
            where TModel : class
        {
            return new ConvertableResult<TModel>(target!);
        }

        public static ConvertableListResult<TModel> ConvertableList<TModel>(this IGraphQlResult<IEnumerable<TModel?>?> target)
            where TModel : class
        {
            return new ConvertableListResult<TModel>(target!);
        }

        public static IGraphQlResult<T> Union<T>(this IGraphQlResult<T> graphQlResult, IGraphQlResult<T> graphQlResult2)
            where T : IEnumerable<IGraphQlResolvable?>?
        {
            var allResults = new List<IGraphQlResult<T>>();
            if (graphQlResult is IUnionGraphQlResult<T> union)
            {
                allResults.AddRange(union.Results);
            }
            else
            {
                allResults.Add(graphQlResult);
            }
            if (graphQlResult2 is IUnionGraphQlResult<T> union2)
            {
                allResults.AddRange(union2.Results);
            }
            else
            {
                allResults.Add(graphQlResult2);
            }

            return new GraphQlUnionResult<T>(allResults);
        }

        public class ConvertableResult<TModel>
        {
            private readonly IGraphQlResult<TModel> target;
            public ConvertableResult(IGraphQlResult<TModel> target) => this.target = target;


            public IGraphQlResult As(Type contract) =>
                GraphQlExpressionResult.Construct(contract, target.UntypedResolver, target.ServiceProvider, target.Joins);

            public IGraphQlResult<TContract> As<TContract>()
                where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable =>
                new GraphQlExpressionResult<TContract>(target.UntypedResolver, target.ServiceProvider, target.Joins);

        }

        public class ConvertableListResult<TModel>
        {
            private readonly IGraphQlResult<IEnumerable<TModel>> target;
            public ConvertableListResult(IGraphQlResult<IEnumerable<TModel>> target) => this.target = target;

            public IGraphQlResult<IEnumerable<TContract>> As<TContract>()
                where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable =>
                new GraphQlExpressionResult<IEnumerable<TContract>>(target.UntypedResolver, target.ServiceProvider, target.Joins);

            public IGraphQlResult<TContract> As<TContract>(Expression<Func<IQueryable<object>, object>> finalizer)
                where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable =>
                new GraphQlExpressionResult<TContract>(target.UntypedResolver, target.ServiceProvider, target.Joins, finalizer);
        }
    }
}
