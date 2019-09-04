using GraphQlResolver.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GraphQlResolver
{
    public static class EndpointRouteBuilderExtensions
    {

        public static IEndpointConventionBuilder UseGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(this Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints, string pattern)
            where TQuery : class, IGraphQlResolvable
            where TMutation : class, IGraphQlResolvable
            where TGraphQlTypeResolver : class, IGraphQlTypeResolver
        {
            return endpoints.MapPost(pattern, async context =>
            {
                var executor = context.RequestServices.GetRequiredService<GraphQlExecutor<TQuery, TMutation, TGraphQlTypeResolver>>();

                object executionResult;
                using (var body = await JsonDocument.ParseAsync(context.Request.Body))
                {
                    var query = body.RootElement.GetProperty("query").GetString();
                    var variables = body.RootElement.TryGetProperty("variables", out var vars) ? vars : (JsonElement?)null;

                    context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("application/json");

                    executionResult = executor.Execute(query, types =>
                        types.ToDictionary(kvp => kvp.Key, kvp => (object?)JsonSerializer.Deserialize(variables?.GetProperty(kvp.Key).GetRawText(), kvp.Value))
                    );
                }
                await JsonSerializer.SerializeAsync(context.Response.Body, (IDictionary<string, object?>)executionResult);
            });
        }
    }
}
