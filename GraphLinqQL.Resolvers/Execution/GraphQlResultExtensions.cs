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
            return new ExecutionResult(result.ErrorDuringParse, await UnrollResults(result.Data, cancellationToken).ConfigureAwait(false), result.Errors);
        }

        private static async Task<object?> UnrollResults(object? data, CancellationToken cancellationToken)
        {
            switch (data)
            {
                case Task task:
                    await task.ConfigureAwait(false);
                    var type = GetTaskType(task.GetType());
                    if (type != null)
                    {
                        return await UnrollResults(GetResultFrom(task, type), cancellationToken).ConfigureAwait(false);
                    }
                    break;
                case IDictionary<string, object?> complex:
                    foreach (var entry in complex.Keys.ToArray())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        complex[entry] = await UnrollResults(complex[entry], cancellationToken).ConfigureAwait(false);
                    }
                    break;
                case IEnumerable<object?> list:
                    var items = list.ToArray();
                    for (var i = 0; i < items.Length; i++)
                    {
                        items[i] = await UnrollResults(items[i], cancellationToken).ConfigureAwait(false);
                    }
                    return items;
                default:
                    break;
            }
            return data;
        }

        private static Type? GetTaskType(Type type)
        {
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return type.GetGenericArguments()[0];
            }
            else if (type.BaseType != null && typeof(Task).IsAssignableFrom(type.BaseType))
            {
                return GetTaskType(type.BaseType);
            }
            return null;
        }

        private static object? GetResultFrom(Task task, Type type)
        {
            return TypedGetResultFromMethod.MakeGenericMethod(type).Invoke(null, new object[] { task });
        }

        public static readonly MethodInfo TypedGetResultFromMethod = typeof(GraphQlResultExtensions).GetMethod(nameof(TypedGetResultFrom), BindingFlags.Static | BindingFlags.NonPublic)!;
        private static object? TypedGetResultFrom<T>(Task<T> task)
        {
            return task.Result;
        }

        internal static ExecutionResult InvokeExpression(object input, LambdaExpression constructedResult)
        {
            var func = Expression.Lambda<Func<object>>(constructedResult.Inline(Expression.Constant(input)));
            // TODO - get errors here
            return new ExecutionResult(false, func.Compile()(), EmptyArrayHelper.Empty<GraphQlError>());
        }
    }
}
