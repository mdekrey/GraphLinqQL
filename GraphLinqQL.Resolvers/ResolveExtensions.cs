using GraphLinqQL.Resolution;
using System;
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
        public static Task<ExecutionResult> GraphQlRootAsync(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult<object>> resolver, Microsoft.Extensions.Logging.ILogger logger, CancellationToken cancellationToken = default)
        {
            var resolved = GetResult<GraphQlRoot>(serviceProvider, contract, resolver).Catch();
            return Execution.GraphQlResultExtensions.InvokeResult(resolved, new GraphQlRoot(), logger, cancellationToken);
        }

        public static IGraphQlScalarResult<object> GetResult<TRoot>(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult<object>> resolver)
        {
            IGraphQlResultFactory<TRoot> resultFactory = new GraphQlResultFactory<TRoot>(FieldContext.Empty);
            var scalar = resultFactory.Resolve(a => a);
            var objectResult = scalar.AsContract<object>(new ContractMapping(contract, typeof(TRoot)), GraphQlContractExpression.ResolveContractIndexed(0));
            var contractBuilder = objectResult.ResolveComplex(serviceProvider);
            var resolved = resolver(contractBuilder);
            return resolved;
        }

        public static IGraphQlScalarResult<TDomainResult> Resolve<TInputType, TDomainResult>(this IGraphQlResultFactory<TInputType> source, TDomainResult result)
        {
            return source.Resolve(_ => result);
        }

        public static IGraphQlObjectResult<IEnumerable<TContractResult>> Union<TInputType, TContractResult>(this IGraphQlResultFactory<TInputType> source, params Func<IGraphQlResultFactory<TInputType>, IGraphQlObjectResult<IEnumerable<TContractResult>>>[] funcs)
            where TContractResult : IGraphQlResolvable?
        {
            var objectResults = funcs.Select(f => f(source)).ToArray();
            var aggregate = objectResults.Aggregate(new 
            { 
                Resolvables = EmptyArrayHelper.Empty<ContractEntry>(), 
                Resolvers = EmptyArrayHelper.Empty<LambdaExpression>(),
            }, (prev, next) =>
            {
                var visitor = new GraphQlContractExpressionReplaceVisitor();
                var param = Expression.Parameter(typeof(object), "unionInput");
                visitor.NewOperations = next.Contract.Resolvables.Select((_, index) => GraphQlContractExpression.ResolveContractIndexed(index + prev.Resolvables.Length)).ToArray();

                var resultLambda = next.Resolution.ApplyVisitor<object>(visitor).ConstructResult();

                return new { Resolvables = prev.Resolvables.Concat(next.Contract.Resolvables).ToArray(), Resolvers = prev.Resolvers.Concat(new[] { resultLambda }).ToArray() };
            });
            return source.AsContract<IEnumerable<TContractResult>>(
                    new ContractMapping(aggregate.Resolvables),
                    ConcatAll(aggregate.Resolvers)
                );
        }

        private static LambdaExpression ConcatAll(IReadOnlyList<LambdaExpression> resolvers)
        {
            var param = resolvers[0].Parameters[0];
            var concat = (Expression<Func<IEnumerable<object>, IEnumerable<object>, IEnumerable<object>>>)((a, b) => Enumerable.Concat(a, b));
            var listResolvers = resolvers.Select(r => (Expression)Expression.Convert(r.Inline(param), typeof(IEnumerable<object>)));

            // assumes each arg is a list
            var fullResolve = listResolvers.Skip(1).Aggregate(listResolvers.First(), (prev, next) => concat.Inline(prev, next));
            return Expression.Lambda(fullResolve, param);
        }

        public static IGraphQlObjectResult<T> AsUnion<T>(this IGraphQlScalarResult graphQlResult, Func<UnionContractBuilder<T>, UnionContractBuilder<T>> contractOptions)
            where T : IGraphQlResolvable
        {
            var builder = contractOptions(new UnionContractBuilder<T>());

            var body = Expression.Parameter(typeof(object), "asUnionInput");

            var temp = builder.Conditions.Select((e, index) => new { index, e.DomainType }).ToArray();
            var bodyWrapper = temp.Length == 1 
                ? GraphQlContractExpression.ResolveContractIndexed(0)
                : Expression.Lambda(
                    temp.Skip(1).Aggregate(
                        GraphQlContractExpression.ResolveContractIndexed(0).Inline(body),
                        (prev, next) => 
                            Expression.Condition(
                                Expression.TypeIs(body, next.DomainType), 
                                GraphQlContractExpression.ResolveContractIndexed(next.index).Inline(body), 
                                prev
                            )
                    ), 
                    body
                );
            return graphQlResult.AsContract<T>(builder.CreateContractMapping(), 
                bodyWrapper
            );
        }

        public static IGraphQlObjectResult<IEnumerable<TContract>> List<TInput, TContract>(this IGraphQlScalarResult<IEnumerable<TInput>> original, Func<IGraphQlScalarResult<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>(original.FieldContext));
            var constructedDeferred = newResult.Resolution.ConstructResult();

            

            var newScalar = original.AddResolve<object>(listExpression =>
            {
                var newResolver = Expression.Lambda(
                    listExpression.CallSelect(constructedDeferred),
                    listExpression
                );
                return newResolver;
            });
            return newResult.AdjustResolution<IEnumerable<TContract>>(_ => newScalar);
        }

        public static IGraphQlObjectResult<TContract> Only<TContract>(this IGraphQlObjectResult<IEnumerable<TContract>> original)
        {
            return GraphQlFinalizerObjectResult<TContract>.Inline(original, (Expression<Func<IEnumerable<object>, object>>)(_ => _.FirstOrDefault()));
        }

        public static IGraphQlScalarResult<TDomainResult> ResolveTask<TInputType, TDomainResult>(this IGraphQlResultFactory<TInputType> original, Func<TInputType, Task<TDomainResult>> resolveAsync)
        {
            var taskFinalizerFactory = new TaskFinalizerFactory(original.FieldContext);

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            Expression<Func<Task<TDomainResult>, IFinalizer>> temp = task => taskFinalizerFactory.Invoke(() => task.ContinueWith(t => PreamblePlaceholders.BodyInvocationPlaceholder(t.Result)));
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
            return original.Resolve(value => resolveAsync(value)).AddResolve<TDomainResult>(temp);
        }

        public static IGraphQlResult ResolveQuery(this IGraphQlResolvable target, string name) =>
            target.ResolveQuery(name, BasicParameterResolver.Empty);

        public static IGraphQlScalarResult<T> AddResolve<T>(this IGraphQlScalarResult result, LambdaExpression resolve)
        {
            return result.AddResolve<T>(_ => resolve);
        }
    }
}
