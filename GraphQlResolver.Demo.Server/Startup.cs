using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQlResolver.Execution;
using GraphQlResolver.StarWarsV3.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace GraphQlResolver.Demo.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddGraphQl<Query, Query, StarWarsV3.Interfaces.TypeResolver>(services);
        }

        private static void AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(IServiceCollection services)
        where TQuery : class, IGraphQlResolvable
        where TMutation : class, IGraphQlResolvable
        where TGraphQlTypeResolver : class, IGraphQlTypeResolver
        {
            services.AddTransient<GraphQlExecutor<TQuery, TMutation, TGraphQlTypeResolver>>();
            services.AddTransient<TQuery>();
            services.AddTransient<TMutation>();
            services.AddTransient<TGraphQlTypeResolver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapPost("/star-wars-v3/graphql", async context =>
                {
                    var executor = context.RequestServices.GetRequiredService<GraphQlExecutor<Query, Query, StarWarsV3.Interfaces.TypeResolver>>();

                    string query;
                    JsonElement? variables;
                    using (var body = await JsonDocument.ParseAsync(context.Request.Body))
                    {
                        query = body.RootElement.GetProperty("query").GetString();
                        variables = body.RootElement.TryGetProperty("variables", out var vars) ? vars : (JsonElement?)null;
                    }

                    context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("application/json");

                    var executionResult = executor.Execute(query, types =>
                        types.ToDictionary(kvp => kvp.Key, kvp => (object?)variables?.GetProperty(kvp.Key))
                    );
                    await JsonSerializer.SerializeAsync(context.Response.Body, executionResult);
                });
            });
        }
    }
}
