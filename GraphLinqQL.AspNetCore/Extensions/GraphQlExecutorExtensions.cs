using GraphLinqQL.ErrorMessages;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GraphLinqQL.Execution
{
    public static class GraphQlExecutorExtensions
    {
        public static async System.Threading.Tasks.Task<Dictionary<string, object?>> ExecuteQuery(this IGraphQlExecutor executor, System.IO.Stream bodyStream, IMessageResolver? messageResolver = null)
        {
            ExecutionResult executionResult;
            using (var body = await JsonDocument.ParseAsync(bodyStream).ConfigureAwait(false))
            {
                var query = body.RootElement.GetProperty("query").GetString();
                var variables = body.RootElement.TryGetProperty("variables", out var vars) ? vars : (JsonElement?)null;


                executionResult = executor.Execute(query, variables?.EnumerateObject().ToDictionary(p => p.Name, p => (IGraphQlParameterInfo)new SystemJsonGraphQlParameterInfo(p.Value)));
            }
            var responseObj = executionResult.ErrorDuringParse
                ? new Dictionary<string, object?>
                {
                        { "errors", BuildErrors(executionResult.Errors) },
                }
                : new Dictionary<string, object?>
                {
                        { "data", executionResult.Data },
                        { "errors", BuildErrors(executionResult.Errors) },
                };
            return responseObj;

            IReadOnlyList<object> BuildErrors(IReadOnlyList<GraphQlError> errors)
            {
                return errors.Select(err => new
                {
                    message = messageResolver?.GetMessage(err.ErrorCode, err.Arguments),
                    errorCode = err.ErrorCode,
                    locations = err.Locations,
                    arguments = err.Arguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                }).ToArray();
            }
        }
    }
}
