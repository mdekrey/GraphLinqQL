using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
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

                var bodyStream = context.Request.Body;
                context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("application/json");
                var responseObj = await executor.ExecuteQuery(bodyStream, messageResolver, context.RequestAborted).ConfigureAwait(false);
                await JsonSerializer.SerializeAsync(context.Response.Body, responseObj, JsonOptions.GraphQlJsonSerializerOptions, context.RequestAborted).ConfigureAwait(false);

            });
        }
    }
}
