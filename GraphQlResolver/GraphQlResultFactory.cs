using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlResultFactory<TValue> : IGraphQlResultFactory<TValue>
    {
        private readonly IServiceProvider serviceProvider;
        public GraphQlResultFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(join);
        }

        IGraphQlResultWithComplexFactory<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return new GraphQlResultResolving<TValue, TDomainResult>(serviceProvider, resolver);
        }

        IGraphQlListResultWithComplexFactory<TModel> IGraphQlResultFactory<TValue>.ResolveList<TModel>(Expression<Func<TValue, IEnumerable<TModel>>> resolver)
        {
            return new GraphQlListResultResolving<TValue, TModel>(serviceProvider, resolver);
        }
    }

    internal class GraphQlResultResolving<TInput, TModel> : IGraphQlResultWithComplexFactory<TModel>, IGraphQlResultFromInput<TInput>
    {
        private Expression<Func<TInput, TModel>> resolver;
        private readonly IServiceProvider serviceProvider;

        public GraphQlResultResolving(IServiceProvider serviceProvider, Expression<Func<TInput, TModel>> resolver)
        {
            this.serviceProvider = serviceProvider;
            this.resolver = resolver;
        }

        IGraphQlComplexResult<TContract> IGraphQlResultWithComplexFactory<TModel>.As<TContract>()
        {
            throw new NotImplementedException();
        }

        Expression<Func<TInput, object>> IGraphQlResultFromInput<TInput>.Resolve()
        {
            return Expressions.ChangeReturnType<TInput, TModel, object>(resolver);
        }
    }

    internal class GraphQlListResultResolving<TInput, TModel> : IGraphQlListResultWithComplexFactory<TModel>, IGraphQlResultFromInput<TInput>
    {
        private readonly Expression<Func<TInput, IEnumerable<TModel>>> resolver;
        private readonly IServiceProvider serviceProvider;

        public GraphQlListResultResolving(IServiceProvider serviceProvider, Expression<Func<TInput, IEnumerable<TModel>>> resolver)
        {
            this.serviceProvider = serviceProvider;
            this.resolver = resolver;
        }

        IGraphQlComplexListResult<TContract> IGraphQlListResultWithComplexFactory<TModel>.As<TContract>()
        {
            return new GraphQlExpressionListResult<TContract, TInput, TModel>(resolver, serviceProvider);
        }

        Expression<Func<TInput, object>> IGraphQlResultFromInput<TInput>.Resolve()
        {
            return Expressions.ChangeReturnType<TInput, IEnumerable<TModel>, object>(resolver);
        }
    }

    internal class GraphQlExpressionListResult<TContract, TInput, TModel> : IGraphQlComplexListResult<TContract>
        where TContract : IGraphQlResolvable, IGraphQlAccepts<TModel>
    {
        private static MethodInfo enumerableSelect = typeof(System.Linq.Enumerable).GetMethods()
            .Where(m => m.Name == nameof(System.Linq.Enumerable.Select))
            .Select(m => m.MakeGenericMethod(typeof(TModel), typeof(IDictionary<string, object>)))
            .Where(m => m.GetParameters()[1].ParameterType == typeof(Func<TModel, IDictionary<string, object>>))
            .Single();
        private readonly Expression<Func<TInput, IEnumerable<TModel>>> resolver;
        private readonly IServiceProvider serviceProvider;

        public GraphQlExpressionListResult(Expression<Func<TInput, IEnumerable<TModel>>> resolver, IServiceProvider serviceProvider)
        {
            this.resolver = resolver;
            this.serviceProvider = serviceProvider;
        }

        IComplexResolverBuilder<TContract, IEnumerable<IDictionary<string, object>>> IGraphQlComplexListResult<TContract>.ResolveComplex()
        {
            var resolver = serviceProvider.GetService<TContract>();
            resolver.Original = new GraphQlResultFactory<TModel>(serviceProvider);
            return new ComplexResolverBuilder<TContract, IEnumerable<IDictionary<string, object>>, TModel>(
                resolver,
                ToListResult
            );
        }

        private IGraphQlResult<IEnumerable<IDictionary<string, object>>> ToListResult(Expression<Func<TModel, IDictionary<string, object>>> expression)
        {
            var inputParameter = Expression.Parameter(typeof(TInput));
            var getList = this.resolver.Body.Replace(this.resolver.Parameters[0], with: inputParameter);

            // TODO - enumerable vs queryable
            var func = Expression.Lambda<Func<TInput, IEnumerable<IDictionary<string, object>>>>(Expression.Call(enumerableSelect, getList, expression), inputParameter);

            return new GraphQlExpressionResult<TInput, IEnumerable<IDictionary<string, object>>>(func, serviceProvider);
        }
    }
}