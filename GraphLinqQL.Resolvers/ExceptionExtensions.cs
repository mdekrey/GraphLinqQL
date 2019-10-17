using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphLinqQL
{
    public static class ExceptionExtensions
    {
        const string errorKey = "GraphQlError";

        public static T AddGraphQlError<T>(this T exception, string errorCode, IReadOnlyList<QueryLocation> queryLocations, object? arguments = null)
            where T : Exception
        {
            exception.Data[errorKey] = ImmutableList.Create(new GraphQlError(errorCode, arguments == null ? ImmutableDictionary<string, object>.Empty : GetArguments(arguments), queryLocations));
            return exception;
        }

        public static bool HasGraphQlErrors(this Exception exception, out ImmutableList<GraphQlError> error)
        {
            var result = exception.Data?.Contains(errorKey) ?? false;
            error = (result ? exception.Data![errorKey] as ImmutableList<GraphQlError> : null) ?? ImmutableList<GraphQlError>.Empty;
            if (exception is AggregateException aggregate)
            {
                var hasAdditional = aggregate.HasGraphQlErrors(out var additional);
                var builder = additional.ToBuilder();
                builder.AddRange(error);
                foreach (var entry in aggregate.InnerExceptions)
                {
                    if (entry.HasGraphQlErrors(out var iterationErrors))
                    {
                        builder.AddRange(iterationErrors);
                    }
                }
                error = builder.ToImmutable();
                return result || hasAdditional;
            }
            else if (exception.InnerException is Exception inner)
            {
                var hasAdditional = inner.HasGraphQlErrors(out var additional);
                var builder = additional.ToBuilder();
                builder.AddRange(error);
                error = builder.ToImmutable();
                return result || hasAdditional;
            }
            else
            {
                return result;
            }
        }

        public static IDictionary<string, object> GetArguments(object arguments)
        {
            var type = arguments.GetType();
            var props = type.GetProperties();
            var result = props.ToImmutableDictionary(p => p.Name, p => p.GetValue(arguments, null));
            return result;
        }
    }
}
