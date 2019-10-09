using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public static class Resolve
    {
        internal static MethodInfo asQueryable = typeof(Queryable).GetMethods()
            .Where(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .Single();

        public static ExecutionResult GraphQlRoot(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlResult> resolver)
        {
            IGraphQlResult resolved = GetResult<GraphQlRoot>(serviceProvider, contract, resolver);
            return Execution.GraphQlResultExtensions.InvokeResult(resolved, new GraphQlRoot());
        }

        public static IGraphQlResult GetResult<TRoot>(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlResult> resolver)
        {
            IGraphQlResultFactory<TRoot> resultFactory = new GraphQlResultFactory<TRoot>();
            var resolved = resolver(resultFactory.Resolve(a => a).AsContract(contract).ResolveComplex(serviceProvider, FieldContext.Empty));
            return resolved;
        }

        public static IGraphQlObjectResult<T> Union<T>(this IGraphQlObjectResult<T> graphQlResult, IGraphQlObjectResult<T> graphQlResult2)
            where T : IEnumerable<IGraphQlResolvable?>?
        {
            var allResults = new List<IGraphQlObjectResult<T>>();
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

        public static IGraphQlScalarResult<IEnumerable<TContract>> List<TInput, TContract>(this IGraphQlScalarResult<IEnumerable<TInput>> original, Func<IGraphQlScalarResult<TInput>, IGraphQlScalarResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            var getList = original.UntypedResolver.Body;
            if (!typeof(IQueryable<>).MakeGenericType(typeof(TInput)).IsAssignableFrom(getList.Type))
            {
                getList = Expression.Call(asQueryable.MakeGenericMethod(typeof(TInput)), getList);
            }

            var newResolver = Expression.Lambda(
                getList.CallQueryableSelect(newResult.UntypedResolver),
                original.UntypedResolver.Parameters
            );
            if (newResult.Joins.Count > 0)
            {
                throw new NotSupportedException($"Inner result of {nameof(Nullable)} cannot provide joins.");
            }
            return new GraphQlExpressionScalarResult<IEnumerable<TContract>>(newResolver, original.Joins);
        }

        public static IGraphQlObjectResult<IEnumerable<TContract>> List<TInput, TContract>(this IGraphQlScalarResult<IEnumerable<TInput>> original, Func<IGraphQlScalarResult<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            var getList = original.UntypedResolver.Body;
            if (!typeof(IQueryable<>).MakeGenericType(typeof(TInput)).IsAssignableFrom(getList.Type))
            {
                getList = Expression.Call(asQueryable.MakeGenericMethod(typeof(TInput)), getList);
            }

            var newResolver = Expression.Lambda(
                getList.CallQueryableSelect(newResult.UntypedResolver),
                original.UntypedResolver.Parameters
            );
            if (newResult.Joins.Count > 0)
            {
                throw new NotSupportedException($"Inner result of {nameof(Nullable)} cannot provide joins.");
            }
            return new GraphQlExpressionObjectResult<IEnumerable<TContract>>(newResolver, newResult.Contract, original.Joins);
        }

        public static IGraphQlScalarResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            return new GraphQlFinalizerScalarResult<TContract>(newResult, 
                newResultResolver => Expression.Lambda(original.UntypedResolver.Body.IfNotNull(newResultResolver.Inline(original.UntypedResolver.Body)), original.UntypedResolver.Parameters));
        }

        public static IGraphQlObjectResult<TContract?> Nullable<TInput, TContract>(this IGraphQlScalarResult<TInput?> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
            where TInput : class
            where TContract : class
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            return new GraphQlFinalizerObjectResult<TContract>(newResult,
                newResultResolver => Expression.Lambda(original.UntypedResolver.Body.IfNotNull(newResultResolver.Inline(original.UntypedResolver.Body)), original.UntypedResolver.Parameters));
        }

        public static IGraphQlScalarResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            return new GraphQlDeferredScalarResult<TContract>(newResult, original);
        }

        public static IGraphQlObjectResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());

            return new GraphQlDeferredObjectResult<TContract>(newResult, original);
        }

        public static IGraphQlObjectResult<TContract> Only<TContract>(this IGraphQlObjectResult<IEnumerable<TContract>> original)
        {
            return GraphQlFinalizerObjectResult<TContract>.Inline(original, (Expression<Func<IEnumerable<object>, object>>)(_ => _.FirstOrDefault()));
        }

        public static IGraphQlScalarResult<Task<TDomainResult>> ResolveTask<TInputType, TDomainResult>(this IGraphQlResultFactory<TInputType> original, Func<TInputType, Task<TDomainResult>> resolveAsync)
        {
            return original.Resolve(value => resolveAsync(value));
        }

        public static IGraphQlObjectResult<TContract> ResolveTask<TInputType, TDomainResult, TContract>(this IGraphQlResultFactory<TInputType> original, Func<TInputType, Task<TDomainResult>> resolveAsync, Func<IGraphQlScalarResult<TDomainResult>, IGraphQlObjectResult<TContract>> func)
        {
            return original.ResolveTask(resolveAsync)
                // FIXME - this should not use .Result if we can help it
                .Defer(r => func(r.Resolve(t => t.Result)));
        }

        public static IGraphQlResult ResolveQuery(this IGraphQlResolvable target, FieldContext fieldContext, string name) =>
            target.ResolveQuery(name, fieldContext, BasicParameterResolver.Empty);
    }
}
