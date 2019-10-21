using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphLinqQL.Execution
{
    public static class GraphQlResultExtensions
    {
        public static async Task<ExecutionResult> InvokeResult(this IGraphQlScalarResult resolved, object input, CancellationToken cancellationToken)
        {
            if (resolved.Joins.Any())
            {
                throw new InvalidOperationException("Cannot join at the root level");
            }
            var constructedResult = resolved.ConstructResult();
            var result = InvokeExpression(input, constructedResult);
            var finalizerContext = new FinalizerContext(UnrollResults, cancellationToken);
            var finalizedResult = await UnrollResults(result, finalizerContext).ConfigureAwait(false);
            return new ExecutionResult(false, finalizedResult, finalizerContext.Errors.ToArray());
        }

        private static async Task<object?> UnrollResults(object? data, FinalizerContext finalizerContext)
        {
            switch (data)
            {
                case IFinalizer finalizer:
                    var result = await finalizer.GetValue(finalizerContext).ConfigureAwait(false);
                    return result;
                case IDictionary<string, object?> complex:
                    foreach (var entry in complex.Keys.ToArray())
                    {
                        finalizerContext.CancellationToken.ThrowIfCancellationRequested();
                        complex[entry] = await UnrollResults(complex[entry], finalizerContext).ConfigureAwait(false);
                    }
                    break;
                case IEnumerable<object?> list:
                    var items = list.ToArray();
                    for (var i = 0; i < items.Length; i++)
                    {
                        items[i] = await UnrollResults(items[i], finalizerContext).ConfigureAwait(false);
                    }
                    return items;
                default:
                    break;
            }
            return data;
        }

        internal static object InvokeExpression(object input, LambdaExpression constructedResult)
        {
            var func = Expression.Lambda<Func<object>>(constructedResult.Inline(Expression.Constant(input)));
            return func.Compile()();
        }
    }
}
