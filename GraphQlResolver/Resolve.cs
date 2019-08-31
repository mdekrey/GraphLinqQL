using GraphQlSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GraphQlResolver
{
    public static class Resolve
    {
        public static IQueryable<T> Query<T>() // TODO - this is a temp function for tests
        {
            return new Query<T>(new GraphQlQueryProvider());
        }

        public static object? GraphQlRoot<T>(this IServiceProvider serviceProvider, Func<IComplexResolverBuilder<T, object>, IGraphQlResult<object>> resolver)
            where T : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        {
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(serviceProvider);
            var resolved = resolver(resultFactory.Resolve(a => a).Convertable().As<T>().ResolveComplex());
            return resolved?.ResolveExpression<GraphQlRoot>().Compile()(new GraphQlRoot());
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

            IGraphQlResult<IDictionary<string, object>> ToResult(LambdaExpression expression)
            {
                var inputParameter = target.UntypedResolver.Parameters[0];
                var func = Expression.Lambda(expression.Body.Replace(expression.Parameters[0], with: inputParameter), inputParameter);
                return new GraphQlExpressionResult<IDictionary<string, object>>(func, target.ServiceProvider);
            }
        }


        public static IComplexResolverBuilder<TContract, IEnumerable<IDictionary<string, object>>> ResolveComplex<TContract>(this IGraphQlResult<IEnumerable<TContract>> target)
            where TContract : IGraphQlResolvable
        {
            var actualContractType = TypeSystem.GetElementType(target.GetType().GetGenericArguments()[0]);
            var actualModelType = TypeSystem.GetElementType(target.UntypedResolver.ReturnType);
            GetContract<TContract>(target, actualContractType, actualModelType, out var resolver, out var modelType);

            var genericArgs = new[] { modelType, typeof(IDictionary<string, object>) };
            var enumerableSelect = typeof(System.Linq.Enumerable).GetMethods()
                .Where(m => m.Name == nameof(System.Linq.Enumerable.Select))
                .Select(m => m.MakeGenericMethod(genericArgs))
                .Where(m => m.GetParameters()[1].ParameterType == typeof(Func<,>).MakeGenericType(genericArgs))
                .Single();

            return new ComplexResolverBuilder<TContract, IEnumerable<IDictionary<string, object>>>(
                resolver,
                ToListResult,
                modelType
            );

            IGraphQlResult<IEnumerable<IDictionary<string, object>>> ToListResult(LambdaExpression expression)
            {
                var inputParameter = target.UntypedResolver.Parameters[0];
                var getList = target.UntypedResolver.Body.Replace(target.UntypedResolver.Parameters[0], with: inputParameter);

                // TODO - enumerable vs queryable
                var func = Expression.Lambda(Expression.Call(enumerableSelect, getList, expression), inputParameter);

                return new GraphQlExpressionResult<IEnumerable<IDictionary<string, object>>>(func, target.ServiceProvider);
            }

        }

        private static void GetContract<TContract>(IGraphQlResult target, Type actualContractType, Type actualModelType, out TContract resolver, out Type modelType) where TContract : IGraphQlResolvable
        {
            resolver = (TContract)target.ServiceProvider.GetService(actualContractType);
            var accepts = (resolver as IGraphQlAccepts);
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

        public static ConvertableResult<TModel> Convertable<TModel>(this IGraphQlResult<TModel> target)
        {
            return new ConvertableResult<TModel>(target);
        }

        public static ConvertableListResult<TModel> ConvertableList<TModel>(this IGraphQlResult<IEnumerable<TModel>> target)
        {
            return new ConvertableListResult<TModel>(target);
        }

        public class ConvertableResult<TModel> : IGraphQlResult<TModel>
        {
            private IGraphQlResult<TModel> target;

            public ConvertableResult(IGraphQlResult<TModel> target)
            {
                this.target = target;
            }

            public IServiceProvider ServiceProvider => target.ServiceProvider;

            public LambdaExpression UntypedResolver => target.UntypedResolver;

            public IGraphQlResult<TContract> As<TContract>()
                where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable
            {
                return new GraphQlComplexResult<TContract, TModel>(target.UntypedResolver, target.ServiceProvider);
            }
        }

        public class ConvertableListResult<TModel> : IGraphQlResult<IEnumerable<TModel>>
        {
            private IGraphQlResult<IEnumerable<TModel>> target;

            public ConvertableListResult(IGraphQlResult<IEnumerable<TModel>> target)
            {
                this.target = target;
            }

            public IServiceProvider ServiceProvider => target.ServiceProvider;

            public LambdaExpression UntypedResolver => target.UntypedResolver;

            public IGraphQlResult<IEnumerable<TContract>> As<TContract>()
                where TContract : IGraphQlAccepts<TModel>, IGraphQlResolvable
            {
                return new GraphQlComplexResult<IEnumerable<TContract>, IEnumerable<TModel>>(target.UntypedResolver, target.ServiceProvider);
            }
        }
    }
}
