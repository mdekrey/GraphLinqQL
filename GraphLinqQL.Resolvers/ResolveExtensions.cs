﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public static class ResolveExtensions
    {
        internal static MethodInfo asQueryable = typeof(Queryable).GetMethods()
            .Where(m => m.Name == nameof(Queryable.AsQueryable) && m.IsGenericMethodDefinition)
            .Single();

        public static Task<ExecutionResult> GraphQlRootAsync(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult> resolver, CancellationToken cancellationToken = default)
        {
            var resolved = GetResult<GraphQlRoot>(serviceProvider, contract, resolver);
            return Execution.GraphQlResultExtensions.InvokeResult(resolved, new GraphQlRoot(), cancellationToken);
        }

        public static IGraphQlScalarResult GetResult<TRoot>(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult> resolver)
        {
            IGraphQlResultFactory<TRoot> resultFactory = new GraphQlResultFactory<TRoot>();
            var resolved = resolver(resultFactory.Resolve(a => a).AsContract<object>(new ContractMapping(contract)).ResolveComplex(serviceProvider, FieldContext.Empty));
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

            return graphQlResult.AsContract<T>(builder.CreateContractMapping());
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
            return newResult.AdjustResolution<IEnumerable<TContract>>(_ => newScalar);
        }

        public static IGraphQlScalarResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.ConstructResult();

            return original.UpdatePreambleAndBody<TContract>(preambleLambda =>
            {
                // FIXME - this probably shouldn't use InvokeExpression, as the errors that are gathered should be propagated.
                Expression<Func<object, LambdaExpression, object?>> newPreamble = (input, deferFunction) => Execution.GraphQlResultExtensions.InvokeExpression(input, deferFunction).Data;
                return Expression.Lambda(newPreamble.Inline(preambleLambda.Body, PreamblePlaceholders.BodyPlaceholderExpression), preambleLambda.Parameters);
            }, deferredLambda => constructedDeferred);
        }

        public static IGraphQlObjectResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdatePreambleAndBody<object>(preambleLambda =>
            {
                // FIXME - this probably shouldn't use InvokeExpression, as the errors that are gathered should be propagated.
                Expression<Func<object, LambdaExpression, object?>> newPreamble = (input, deferFunction) => Execution.GraphQlResultExtensions.InvokeExpression(input, deferFunction).Data;
                return Expression.Lambda(newPreamble.Inline(preambleLambda.Body, PreamblePlaceholders.BodyPlaceholderExpression), preambleLambda.Parameters);
            }, deferredLambda => constructedDeferred);
            return newResult.AdjustResolution<TContract>(_ => newScalar);
        }

        public static IGraphQlObjectResult<TContract> Only<TContract>(this IGraphQlObjectResult<IEnumerable<TContract>> original)
        {
            return GraphQlFinalizerObjectResult<TContract>.Inline(original, (Expression<Func<IEnumerable<object>, object>>)(_ => _.FirstOrDefault()));
        }

        public static IGraphQlScalarResult<TDomainResult> ResolveTask<TInputType, TDomainResult>(this IGraphQlResultFactory<TInputType> original, Func<TInputType, Task<TDomainResult>> resolveAsync)
        {
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            Expression<Func<Task<TDomainResult>, Task>> temp = task => task.ContinueWith(t => PreamblePlaceholders.BodyInvocationPlaceholder(t.Result));
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
            return original.Resolve(value => resolveAsync(value))
                .UpdatePreamble<TDomainResult>(preamble => Expression.Lambda(temp.Inline(preamble.Body), preamble.Parameters));
        }

        public static IGraphQlResult ResolveQuery(this IGraphQlResolvable target, FieldContext fieldContext, string name) =>
            target.ResolveQuery(name, fieldContext, BasicParameterResolver.Empty);
    }
}
