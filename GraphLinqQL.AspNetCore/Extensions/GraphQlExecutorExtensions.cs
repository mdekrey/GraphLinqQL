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
                var operationName = body.RootElement.TryGetProperty("operationName", out var op) ? op.GetString() : null;
                var variables = body.RootElement.TryGetProperty("variables", out var vars) ? vars : (JsonElement?)null;


                executionResult = executor.Execute(query, operationName, variables?.EnumerateObject().ToDictionary(p => p.Name, p => (IGraphQlParameterInfo)new SystemJsonGraphQlParameterInfo(p.Value)));
            }
            var responseObj = new Dictionary<string, object?>();
            if (!executionResult.ErrorDuringParse)
            {
                responseObj["data"] = executionResult.Data;
            }
            if (executionResult.Errors != null && executionResult.Errors.Any())
            {
                responseObj["errors"] = BuildErrors(executionResult.Errors);
            }
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
