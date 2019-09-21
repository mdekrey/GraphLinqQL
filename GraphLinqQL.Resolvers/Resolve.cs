﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    public static class Resolve
    {
        internal static MethodInfo asQueryable = typeof(Queryable).GetMethods()
            .Where(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .Single();

        public static object GraphQlRoot(this IGraphQlServiceProvider serviceProvider, Type t, Func<IComplexResolverBuilder, IGraphQlResult> resolver)
        {
            var parameterResolverFactory = serviceProvider.GetParameterResolverFactory();
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(parameterResolverFactory);
            var resolved = resolver(resultFactory.Resolve(a => a).As(t).ResolveComplex(serviceProvider));
            var expression = resolved.UntypedResolver.CastAndBoxSingleInput<GraphQlRoot>();
            var queryable = Enumerable.Repeat(new GraphQlRoot(), 1).AsQueryable().Select(expression);
            return queryable.Single();
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

            return new GraphQlUnionResult<T>(graphQlResult.ParameterResolverFactory, allResults);
        }

        public static IGraphQlResult<IEnumerable<TContract>> List<TInput, TContract>(this IGraphQlResult<IEnumerable<TInput>> original, Func<IGraphQlResult<TInput>, IGraphQlResult<TContract>> func)
        {
            if (original.Contract != null)
            {
                throw new ArgumentException($"Original cannot already have a contract, but had {original.Contract.FullName}.");
            }
            else if (original.Finalizer != null)
            {
                throw new ArgumentException($"Original cannot already have a finalizer.");
            }
            var newResult = func(new GraphQlResultFactory<TInput>(original.ParameterResolverFactory));

            var getList = original.UntypedResolver.Body;
            if (!typeof(IQueryable<>).MakeGenericType(typeof(TInput)).IsAssignableFrom(getList.Type))
            {
                getList = Expression.Call(asQueryable.MakeGenericMethod(typeof(TInput)), getList);
            }

            var newResolver = Expression.Lambda(
                Expressions.CallQueryableSelect(getList, newResult.UntypedResolver), 
                original.UntypedResolver.Parameters
            );
            if (newResult.Joins.Count > 0)
            {
                throw new NotSupportedException($"Inner result of {nameof(Nullable)} cannot provide joins.");
            }
            return new GraphQlExpressionResult<IEnumerable<TContract>>(original.ParameterResolverFactory, newResolver, newResult.Contract, original.Joins);
        }

        public static IGraphQlResult<TContract?> Nullable<TInput, TContract>(this IGraphQlResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            if (original.Contract != null)
            {
                throw new ArgumentException($"Original cannot already have a contract, but had {original.Contract.FullName}.");
            }
            else if (original.Finalizer != null)
            {
                throw new ArgumentException($"Original cannot already have a finalizer.");
            }
            var newResult = func(new GraphQlResultFactory<TInput>(original.ParameterResolverFactory));

            var newResolver = Expression.Lambda(original.UntypedResolver.Body.IfNotNull(newResult.UntypedResolver.Inline(original.UntypedResolver.Body)), original.UntypedResolver.Parameters);
            if (newResult.Joins.Count > 0)
            {
                throw new NotSupportedException($"Inner result of {nameof(Nullable)} cannot provide joins.");
            }
            return new GraphQlExpressionResult<TContract?>(original.ParameterResolverFactory, newResolver, newResult.Contract, original.Joins, newResult.Finalizer);
        }

        public static IGraphQlResult<TContract> Only<TContract>(this IGraphQlResult<IEnumerable<TContract>> original)
        {
            return new GraphQlExpressionResult<TContract>(original.ParameterResolverFactory, original.UntypedResolver, original.Contract, original.Joins, (Expression<Func<IEnumerable<object>, object>>)(_ => _.FirstOrDefault()));
        }

        public static IGraphQlResult ResolveQuery(this IGraphQlResolvable target, string name) =>
            target.ResolveQuery(name, new NoParameterResolver());
    }
}