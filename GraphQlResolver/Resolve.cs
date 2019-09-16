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

        public static object GraphQlRoot(this IServiceProvider serviceProvider, Type t, Func<IComplexResolverBuilder<object>, IGraphQlResult<object>> resolver)
        {
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(serviceProvider);
            var resolved = resolver(resultFactory.Resolve(a => a).Convertable().As(t).ResolveComplex());
            var expression = resolved?.ResolveExpression<GraphQlRoot>();
            // TODO - check for JOINs
            var queryable = Enumerable.Repeat(new GraphQlRoot(), 1).AsQueryable().Select(expression);
            return queryable.Single();
        }

        public static object GraphQlRoot<T>(this IServiceProvider serviceProvider, Func<IComplexResolverBuilder<T, object>, IGraphQlResult<object>> resolver)
            where T : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        {
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(serviceProvider);
            var resolved = resolver(resultFactory.Resolve(a => a).Convertable().As<T>().ResolveComplex());
            var expression = resolved?.ResolveExpression<GraphQlRoot>();
            var queryable = Enumerable.Repeat(new GraphQlRoot(), 1).AsQueryable().Select(expression);
            return queryable.Single();
        }

        private static readonly IReadOnlyList<MethodInfo> complexResolvers = (from method in typeof(Resolve).GetMethods()
                                                                              where method.Name == nameof(ResolveComplex) && method.IsGenericMethodDefinition
                                                                              select method).ToArray();

        public static IComplexResolverBuilder<object> ResolveComplex(this IGraphQlResult target)
        {
            var paramType = target.GetType();
            var resultType = paramType.GetInterfaces().Where(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IGraphQlResult<>)).Select(iface => iface.GetGenericArguments()[0]).First();
            var contractType = TypeSystem.GetElementType(resultType) ?? resultType;

            if (typeof(IGraphQlResolvable).IsAssignableFrom(contractType))
            {
                var method = complexResolvers.Select(m => m.MakeGenericMethod(contractType)).First(m => m.GetParameters().Single().ParameterType.IsAssignableFrom(paramType));
                return (IComplexResolverBuilder<object>)method.Invoke(null, new[] { target });
            }
            throw new ArgumentException("Not a resolvable complex type", nameof(target));
        }

        public static IComplexResolverBuilder<TContract, IDictionary<string, object>> ResolveComplex<TContract>(this IGraphQlResult<TContract> target)
            where TContract : IGraphQlResolvable
        {
            var actualContractType = target.GetType().GetGenericArguments()[0];
            
            GetContract<TContract>(target, actualContractType, target.UntypedResolver.ReturnType, out var resolver, out var modelType);

            return new ComplexResolverBuilder<TContract, IDictionary<string, object>>(
                resolver,
                ToResult,
                modelType
            );

            IGraphQlResult<IDictionary<string, object>> ToResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
            {
                var inputParameter = target.UntypedResolver.Parameters[0];

                //if (joins.Count > 0)
                {
                    var actualModelType = typeof(TContract).GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IGraphQlAccepts<>)).Single()
                        .GetGenericArguments()[0];

                    var repeat = typeof(Enumerable).GetMethod(nameof(Enumerable.Repeat)).MakeGenericMethod(target.UntypedResolver.ReturnType);
                    var single = typeof(Enumerable).GetMethods().Single(m => m.Name == nameof(Enumerable.Single) && m.GetParameters().Length == 1)
                        .MakeGenericMethod(typeof(IDictionary<string, object>));
                    var convert = Resolve.asQueryable.MakeGenericMethod(target.UntypedResolver.ReturnType);

                    var func = BuildListLambdaWithJoins(
                        Expression.Lambda(Expression.Call(null, convert, Expression.Call(null, repeat, target.UntypedResolver.Body, Expression.Constant(1))), inputParameter), resultSelector, joins, actualModelType);
                    var resultFunc = Expression.Lambda(Expression.Call(null, single, func.Body), func.Parameters);

                    return new GraphQlExpressionResult<IDictionary<string, object>>(resultFunc, target.ServiceProvider, target.Joins);
                }
                //else
                //{

                //    var func = Expression.Lambda(resultSelector.Body.Replace(resultSelector.Parameters[0], with: target.UntypedResolver.Body), inputParameter);
                //    return new GraphQlExpressionResult<IDictionary<string, object>>(func, target.ServiceProvider, target.Joins);
                //}
            }
        }

        public static IComplexResolverBuilder<TContract, IEnumerable<IDictionary<string, object>>> ResolveComplex<TContract>(this IGraphQlResult<IEnumerable<TContract>> target)
            where TContract : IGraphQlResolvable
        {
            if (target is IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult)
            {
                if (!typeof(IGraphQlResolvable).IsAssignableFrom(typeof(TContract)))
                {
                    throw new InvalidOperationException($"Union types can only handle {typeof(IGraphQlResolvable).FullName}");
                }
                return new UnionResolverBuilder<TContract>(unionResult);
            }
            var actualContractType = TypeSystem.GetElementType(target.GetType().GetGenericArguments()[0]);
            var actualModelType = TypeSystem.GetElementType(target.UntypedResolver.ReturnType);
            GetContract<TContract>(target, actualContractType, actualModelType, out var resolver, out var modelType);

            return new ComplexResolverBuilder<TContract, IEnumerable<IDictionary<string, object>>>(
                resolver,
                ToListResult,
                modelType
            );

            IGraphQlResult<IEnumerable<IDictionary<string, object>>> ToListResult(LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins)
            {
                var func = BuildListLambdaWithJoins(target.UntypedResolver, resultSelector, joins, actualModelType);

                return new GraphQlExpressionResult<IEnumerable<IDictionary<string, object>>>(func, target.ServiceProvider, target.Joins);
            }

        }

        private static LambdaExpression BuildListLambdaWithJoins(LambdaExpression targetQueryable, LambdaExpression resultSelector, ImmutableHashSet<IGraphQlJoin> joins, Type actualModelType)
        {
            var inputParameter = targetQueryable.Parameters[0];
            var originalGetList = targetQueryable.Body;
            // TODO - maybe store the list originalGetList in a variable to reuse in condition?
            var getList = originalGetList;
            var modelType = TypeSystem.GetElementType(getList.Type);

            if (!typeof(IQueryable<>).MakeGenericType(modelType).IsAssignableFrom(getList.Type))
            {
                getList = Expression.Call(Resolve.asQueryable.MakeGenericMethod(modelType), getList);
            }

            var originalParameter = Expression.Parameter(modelType, "Original " + modelType.FullName);
            var parameters = new Dictionary<Expression, Expression>(joins.Count) { { originalParameter, originalParameter } };
            var rootParameter = originalParameter;

            if (joins.Count > 0)
            {
                var placeholderType = typeof(JoinPlaceholder<>).MakeGenericType(actualModelType);
                rootParameter = Expression.Parameter(placeholderType, "JoinPlaceholder " + modelType.FullName);
                var originalConstructor = placeholderType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => c.GetParameters().Length == 1).Single();
                getList = Expressions.CallQueryableSelect(getList, Expression.Lambda(Expression.New(originalConstructor, originalParameter), originalParameter));
                parameters[originalParameter] = Expression.Property(rootParameter, nameof(JoinPlaceholder<object>.Original));

                // TODO - could do this all simultaneously with a specialized visitor
                foreach (var join in joins)
                {
                    getList = getList.MergeJoin(rootParameter, join, parameters);
                }
            }

            var mainBody = Expression.Lambda(
                parameters.Aggregate(
                    resultSelector.Body.Replace(resultSelector.Parameters[0], parameters[originalParameter]),
                    (e, parameters) => e.Replace(parameters.Key, parameters.Value)
                ),
                rootParameter
            );
            var selected = Expressions.CallQueryableSelect(getList, mainBody);
            var returnResult = Expression.TryCatch(selected, Expression.Catch(Expression.Parameter(typeof(Exception)), Expression.Constant(null, selected.Type)));

            var func = Expression.Lambda(returnResult, inputParameter);
            return func;
        }

        private static void GetContract<TContract>(IGraphQlResult target, Type actualContractType, Type actualModelType, out TContract resolver, out Type modelType) where TContract : IGraphQlResolvable
        {
            resolver = (TContract)ActivatorUtilities.GetServiceOrCreateInstance(target.ServiceProvider, actualContractType);
            var accepts = resolver as IGraphQlAccepts;
            if (accepts == null)
            {
                throw new ArgumentException("Contract does not accept an input type");
            }
            modelType = accepts.ModelType;
            if (modelType != actualModelType)
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
        }
    }
}
