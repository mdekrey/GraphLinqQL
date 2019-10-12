using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public class TaskFinalizer : IFinalizer
    {
        public static readonly ConstructorInfo ConstructorInfo = typeof(TaskFinalizer).GetConstructors().Single();

        private readonly Task task;

        public TaskFinalizer(Task task)
        {
            this.task = task;
        }

        public async Task<object?> GetValue(FinalizerContext context)
        {
            await task.ConfigureAwait(false);

            var type = GetTaskType(task.GetType());
            if (type != null)
            {
                return GetResultFrom(task, type);
            }
            return null;
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
