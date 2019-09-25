using GraphLinqQL.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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
                context.Response.RegisterForDispose(executor);
                
                object executionResult;
                using (var body = await JsonDocument.ParseAsync(context.Request.Body).ConfigureAwait(false))
                {
                    var query = body.RootElement.GetProperty("query").GetString();
                    var variables = body.RootElement.TryGetProperty("variables", out var vars) ? vars : (JsonElement?)null;

                    context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("application/json");

                    executionResult = executor.Execute(query, variables?.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetRawText()));
                }
                await JsonSerializer.SerializeAsync(context.Response.Body, (IDictionary<string, object?>)executionResult, JsonOptions).ConfigureAwait(false);
            });
        }
    }
}
