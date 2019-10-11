using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public static class ResolveExtensions
    {
        internal static MethodInfo asQueryable = typeof(Queryable).GetMethods()
            .Where(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .Single();

        public static ExecutionResult GraphQlRoot(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult> resolver)
        {
            var resolved = GetResult<GraphQlRoot>(serviceProvider, contract, resolver);
            return Execution.GraphQlResultExtensions.InvokeResult(resolved, new GraphQlRoot());
        }

        public static IGraphQlScalarResult GetResult<TRoot>(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult> resolver)
        {
            IGraphQlResultFactory<TRoot> resultFactory = new GraphQlResultFactory<TRoot>();
            var resolved = resolver(resultFactory.Resolve(a => a).AsContract(contract).ResolveComplex(serviceProvider, FieldContext.Empty));
            return resolved;
        }

        public static IGraphQlScalarResult<TDomainResult> Resolve<TInputType, TDomainResult>(this IGraphQlResultFactory<TInputType> source, TDomainResult result)
        {
            return source.Resolve(_ => result);
        }

        public static IGraphQlObjectResult<T> AsUnion<T>(this IGraphQlScalarResult graphQlResult, Func<UnionContractBuilder<T>, UnionContractBuilder<T>> contractOptions)
            where T : IGraphQlResolvable
        {
            var builder = contractOptions(new UnionContractBuilder<T>());

            return null;
        }

        public static IGraphQlObjectResult<IEnumerable<TContract>> List<TInput, TContract>(this IGraphQlScalarResult<IEnumerable<TInput>> original, Func<IGraphQlScalarResult<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdateBody<object>(getListLamba =>
            {
                var getList = getListLamba.Body;
                if (!typeof(IQueryable<>).MakeGenericType(typeof(TInput)).IsAssignableFrom(getList.Type))
                {
                    getList = Expression.Call(asQueryable.MakeGenericMethod(typeof(TInput)), getList);
                }

                var newResolver = Expression.Lambda(
                    getList.CallQueryableSelect(constructedDeferred),
                    getListLamba.Parameters
                );
                return newResolver;
            });
            return new GraphQlExpressionObjectResult<IEnumerable<TContract>>(newScalar, newResult.Contract);
        }

        public static IGraphQlScalarResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.ConstructResult();

            return original.UpdatePreambleAndBody<TContract>(preambleLambda =>
            {
                Expression<Func<object, LambdaExpression, object?>> newPreamble = (input, deferFunction) => Execution.GraphQlResultExtensions.InvokeExpression(input, deferFunction).Data;
                return Expression.Lambda(newPreamble.Inline(preambleLambda.Body, GraphQlPreambleExpressionReplaceVisitor.BodyPlaceholderExpression), preambleLambda.Parameters);
            }, deferredLambda => constructedDeferred);
        }

        public static IGraphQlObjectResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdatePreambleAndBody<object>(preambleLambda =>
            {
                Expression<Func<object, LambdaExpression, object?>> newPreamble = (input, deferFunction) => Execution.GraphQlResultExtensions.InvokeExpression(input, deferFunction).Data;
                return Expression.Lambda(newPreamble.Inline(preambleLambda.Body, GraphQlPreambleExpressionReplaceVisitor.BodyPlaceholderExpression), preambleLambda.Parameters);
            }, deferredLambda => constructedDeferred);
            return new GraphQlExpressionObjectResult<TContract>(newScalar, newResult.Contract);
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
            return original.Resolve(value => resolveAsync(value))
                // FIXME - this should not use .Result if we can help it
                .Defer(r => func(r.Resolve(t => t.Result)));
        }

        public static IGraphQlResult ResolveQuery(this IGraphQlResolvable target, FieldContext fieldContext, string name) =>
            target.ResolveQuery(name, fieldContext, BasicParameterResolver.Empty);
    }
}
