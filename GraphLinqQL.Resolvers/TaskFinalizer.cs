using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    public class CatchFinalizerFactory
    {
        public static readonly MethodInfo CatchMethodInfo = typeof(CatchFinalizerFactory).GetMethod(nameof(Catch));
        private readonly FieldContext fieldContext;

        public CatchFinalizerFactory(FieldContext fieldContext)
        {
            this.fieldContext = fieldContext;
        }

        public IFinalizer Catch(Func<object> valueAccessor)
        {
            return new CatchFinalizer(fieldContext, valueAccessor);
        }
    }

    public class CatchFinalizer : IFinalizer
    {
        private readonly FieldContext fieldContext;
        private readonly Func<object> valueAccessor;

        public CatchFinalizer(FieldContext fieldContext, Func<object> valueAccessor)
        {
            this.fieldContext = fieldContext;
            this.valueAccessor = valueAccessor;
        }

        public Task<object?> GetValue(FinalizerContext context)
        {
            try
            {
                return Task.FromResult<object?>(valueAccessor());
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                var errors = ex.HasGraphQlErrors(out var error) ? (IReadOnlyList<GraphQlError>)error : new[] { new GraphQlError(WellKnownErrorCodes.UnhandledError, ExceptionExtensions.GetArguments(new { fieldName = fieldContext.Name, type = fieldContext.TypeName }), fieldContext.Locations) };
                context.Errors.AddRange(errors);
                return Task.FromResult<object?>(null);
            }
        }
    }
}
