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
        public static Task<ExecutionResult> GraphQlRootAsync(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult> resolver, CancellationToken cancellationToken = default)
        {
            var resolved = GetResult<GraphQlRoot>(serviceProvider, contract, resolver);
            return Execution.GraphQlResultExtensions.InvokeResult(resolved, new GraphQlRoot(), cancellationToken);
        }

        public static IGraphQlScalarResult GetResult<TRoot>(this IGraphQlServiceProvider serviceProvider, Type contract, Func<IComplexResolverBuilder, IGraphQlScalarResult> resolver)
        {
            IGraphQlResultFactory<TRoot> resultFactory = new GraphQlResultFactory<TRoot>();
            var contractBuilder = resultFactory
                .Resolve(a => a).AsContract<object>(new ContractMapping(contract, typeof(TRoot)), body => GraphQlContractExpression.ResolveContract(body, 0))
                .ResolveComplex(serviceProvider, FieldContext.Empty);
            var resolved = resolver(contractBuilder);
            return resolved;
        }

        public static IGraphQlScalarResult<TDomainResult> Resolve<TInputType, TDomainResult>(this IGraphQlResultFactory<TInputType> source, TDomainResult result)
        {
            return source.Resolve(_ => result);
        }

        public static IGraphQlObjectResult<IEnumerable<TContractResult>> Union<TInputType, TContractResult>(this IGraphQlResultFactory<TInputType> source, params Func<IGraphQlResultFactory<TInputType>, IGraphQlObjectResult<IEnumerable<TContractResult>>>[] funcs)
        {
            var objectResults = funcs.Select(f => f(source)).ToArray();
            var aggregate = objectResults.Aggregate(new 
            { 
                Resolvables = EmptyArrayHelper.Empty<ContractEntry>(), 
                Resolvers = EmptyArrayHelper.Empty<LambdaExpression>(),
            }, (prev, next) =>
            {
                var lambda = next.Resolution.ConstructResult();

                var visitor = new GraphQlContractExpressionReplaceVisitor();
                var param = Expression.Parameter(typeof(object));
                visitor.NewOperations = next.Contract.Resolvables.Select((_, index) => Expression.Lambda(GraphQlContractExpression.ResolveContract(param, index + prev.Resolvables.Length), param)).ToArray();
                var resultLambda = (LambdaExpression)visitor.Visit(lambda);

                return new { Resolvables = prev.Resolvables.Concat(next.Contract.Resolvables).ToArray(), Resolvers = prev.Resolvers.Concat(new[] { resultLambda }).ToArray() };
            });
            return source.AsContract<IEnumerable<TContractResult>>(
                    new ContractMapping(aggregate.Resolvables),
                    _ => ConcatAll(aggregate.Resolvers).Inline(_)
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

            var temp = builder.Conditions.Select((e, index) => new { index, e.DomainType }).ToArray();
            return graphQlResult.AsContract<T>(builder.CreateContractMapping(), body =>
                temp.Length == 1 ? GraphQlContractExpression.ResolveContract(body, 0) :
                    temp.Skip(1).Aggregate(GraphQlContractExpression.ResolveContract(Expression.Convert(body, temp[0].DomainType), 0),
                        (prev, next) => Expression.Condition(Expression.TypeIs(body, next.DomainType), GraphQlContractExpression.ResolveContract(Expression.Convert(body, next.DomainType), next.index), prev))
            );
        }

        public static IGraphQlObjectResult<IEnumerable<TContract>> List<TInput, TContract>(this IGraphQlScalarResult<IEnumerable<TInput>> original, Func<IGraphQlScalarResult<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>());
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdateBody<object>(getListLamba =>
            {
                var getList = getListLamba.Body;
                var newResolver = Expression.Lambda(
                    getList.CallSelect(constructedDeferred),
                    getListLamba.Parameters
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
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            Expression<Func<Task<TDomainResult>, Task>> temp = task => task.ContinueWith(t => PreamblePlaceholders.BodyInvocationPlaceholder(t.Result));
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
            return original.Resolve(value => resolveAsync(value))
                .UpdatePreamble<TDomainResult>(preamble => Expression.Lambda(Expression.New(TaskFinalizer.ConstructorInfo, temp.Inline(preamble.Body)), preamble.Parameters));
        }

        public static IGraphQlResult ResolveQuery(this IGraphQlResolvable target, FieldContext fieldContext, string name) =>
            target.ResolveQuery(name, fieldContext, BasicParameterResolver.Empty);
    }
}
