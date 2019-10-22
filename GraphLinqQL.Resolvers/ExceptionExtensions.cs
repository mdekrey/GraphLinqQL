using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL
{
    public static class ExceptionExtensions
    {
        const string errorKey = "GraphQlError";

        public static T AddGraphQlErrors<T>(this T exception, IReadOnlyList<GraphQlError> errors)
            where T : Exception
        {
            exception.Data[errorKey] = errors;
            return exception;
        }

        public static T AddGraphQlError<T>(this T exception, string errorCode, IReadOnlyList<QueryLocation>? queryLocations = null, object? arguments = null)
            where T : Exception
        {
            return exception.AddGraphQlErrors(new[] { new GraphQlError(errorCode, arguments == null ? null : GetArguments(arguments), queryLocations) });
        }

        public static T AddGraphQlError<T>(this T exception, string errorCode, FieldContext fieldContext)
            where T : Exception
        {
            var error = new GraphQlError(errorCode);
            error.Fixup(fieldContext);
            exception.Data[errorKey] = new[] { error };
            return exception;
        }

        public static bool HasGraphQlErrors(this Exception exception, out IReadOnlyList<GraphQlError> error)
        {
            var result = exception.Data?.Contains(errorKey) ?? false;
            error = (result ? exception.Data![errorKey] as IReadOnlyList<GraphQlError> : null) ?? EmptyArrayHelper.Empty<GraphQlError>();
            if (error.Count > 0)
            {
                return result;
            }
            if (exception is AggregateException aggregate)
            {
                var hasAdditional = aggregate.HasGraphQlErrors(out var additional);
                var builder = additional.ToList();
                builder.AddRange(error);
                foreach (var entry in aggregate.InnerExceptions)
                {
                    if (entry.HasGraphQlErrors(out var iterationErrors))
                    {
                        builder.AddRange(iterationErrors);
                    }
                }
                error = builder.ToArray();
                return result || hasAdditional;
            }
            else if (exception.InnerException is Exception inner)
            {
                var hasAdditional = inner.HasGraphQlErrors(out var additional);
                var builder = additional.ToList();
                builder.AddRange(error);
                error = builder.ToArray();
                return result || hasAdditional;
            }
            else
            {
                return result;
            }
        }

        public static Dictionary<string, object> GetArguments(object arguments)
        {
            var type = arguments.GetType();
            var props = type.GetProperties();
            var result = props.ToDictionary(p => p.Name, p => p.GetValue(arguments, null));
            return result;
        }

        public static void Fixup(this GraphQlError error, FieldContext fieldContext)
        {
            if (error.Locations.Count == 0)
            {
                error.Locations.AddRange(fieldContext.Locations);
                if (fieldContext.Name != null && !error.Arguments.ContainsKey("fieldName"))
                {
                    error.Arguments["fieldName"] = fieldContext.Name;
                }
                if (fieldContext.TypeName != null && !error.Arguments.ContainsKey("type"))
                {
                    error.Arguments["type"] = fieldContext.TypeName;
                }
            }
        }
    }
}
