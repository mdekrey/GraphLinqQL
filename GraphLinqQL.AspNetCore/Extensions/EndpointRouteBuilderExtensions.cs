using GraphLinqQL;
using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static IEndpointConventionBuilder UseGraphQl(this IEndpointRouteBuilder endpoints, string pattern)
        {
            return endpoints.UseGraphQl(pattern, Options.DefaultName);
        }

        public static IEndpointConventionBuilder UseGraphQl(this IEndpointRouteBuilder endpoints, string pattern, string name)
        {
            return endpoints.MapPost(pattern, async context =>
            {
                var executor = context.RequestServices.GetRequiredService<IGraphQlExecutorFactory>().Create(name);
                var messageResolver = context.RequestServices.GetRequiredService<IMessageResolver>();
                context.Response.RegisterForDispose(executor);
                
                ExecutionResult executionResult;
                using (var body = await JsonDocument.ParseAsync(context.Request.Body).ConfigureAwait(false))
                {
                    var query = body.RootElement.GetProperty("query").GetString();
                    var variables = body.RootElement.TryGetProperty("variables", out var vars) ? vars : (JsonElement?)null;

                    context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("application/json");

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
                await JsonSerializer.SerializeAsync(context.Response.Body, responseObj, JsonOptions, context.RequestAborted).ConfigureAwait(false);

                IReadOnlyList<object> BuildErrors(IReadOnlyList<GraphQlError> errors)
                {
                    return errors.Select(err => new
                    {
                        message = messageResolver.GetMessage(err.ErrorCode, err.Arguments),
                        errorCode = err.ErrorCode,
                        locations = err.Locations,
                        arguments = err.Arguments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    }).ToArray();
                }
            });
        }
    }
}
