using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    internal class TaskFinalizer : IFinalizer
    {
        private readonly FieldContext fieldContext;
        private readonly Func<Task> taskFactory;

        internal TaskFinalizer(FieldContext fieldContext, Func<Task> taskFactory)
        {
            this.fieldContext = fieldContext;
            this.taskFactory = taskFactory;
        }

        public async Task<object?> GetValue(FinalizerContext context)
        {
            try
            {
                var task = taskFactory();
                await task.ConfigureAwait(false);

                var type = GetTaskType(task.GetType());
                if (type != null)
                {
                    var result = GetResultFrom(task, type);
                    return await context.UnrollResults(result, context).ConfigureAwait(false);
                }
                return null;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                var errors = ex.HasGraphQlErrors(out var e) ? e : new[] { new GraphQlError(WellKnownErrorCodes.UnhandledError) };
                foreach (var error in errors)
                {
                    error.Fixup(fieldContext);
                }
                throw new InvalidOperationException("Caught error in task", ex).AddGraphQlErrors(errors);
            }
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

        public static readonly MethodInfo TypedGetResultFromMethod = typeof(TaskFinalizer).GetMethod(nameof(TypedGetResultFrom), BindingFlags.Static | BindingFlags.NonPublic)!;
        private static object? TypedGetResultFrom<T>(Task<T> task)
        {
            return task.Result;
        }

    }
}
