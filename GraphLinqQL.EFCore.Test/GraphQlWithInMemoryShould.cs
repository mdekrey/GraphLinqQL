using GraphLinqQL.Ast;
using GraphLinqQL.CodeGeneration;
using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
using GraphLinqQL.StarWars.Domain;
using GraphLinqQL.TestFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace GraphLinqQL
{
    public sealed class GraphQlWithInMemoryShould : IDisposable
    {
        private readonly ServiceProvider serviceProvider;

        public GraphQlWithInMemoryShould()
        {
            var services = new ServiceCollection();
            services.AddDbContext<StarWarsContext>(options => options.UseInMemoryDatabase(nameof(GraphQlWithInMemoryShould) + nameof(StarWarsContext)));
            services.AddDbContext<Blogs.Data.BloggingContext>(options => options.UseInMemoryDatabase(nameof(GraphQlWithInMemoryShould) + nameof(Blogs.Data.BloggingContext)));
            services.AddGraphQl<StarWars.Interfaces.TypeResolver>("star-wars", typeof(StarWars.Implementations.Query), options =>
            {
                options.Mutation = typeof(StarWars.Implementations.Mutation);
                options.AddIntrospection();
            });
            services.AddGraphQl<Edgy.Interfaces.TypeResolver>("edgy", typeof(Edgy.Implementations.Query), options =>
            {
                options.AddIntrospection();
            });
            services.AddGraphQl<Blogs.Api.TypeResolver>("blogs", typeof(Blogs.Api.QueryResolver), options =>
            {
                options.AddIntrospection();
            });
            services.AddLogging();
            serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<StarWarsContext>().Database.EnsureCreated();
            scope.ServiceProvider.GetRequiredService<Blogs.Data.BloggingContext>().Database.EnsureCreated();
        }

        [Theory]
        [YamlScenarios("GraphLinqQL.Scenarios.")]
        public async Task Handle(ScenarioDataNode<Given, When, Then> scenario)
        {
            using var scope = serviceProvider.CreateScope();
            switch (scenario)
            {
                case { Given: { Query: var query }, When: { Parse: true }, Then: { Passes: true } }:
                    await ParseSuccess(scope.ServiceProvider, query);
                    return;
                case { Given: { Query: var query }, When: { Parse: true }, Then: { Passes: false } }:
                    await DetectParsingErrors(scope.ServiceProvider, query);
                    return;
                case { Given: { Query: var query }, When: { CodeGeneration: true, LanguageVersion: var languageVersion, Namespace: var ns }, Then: { Passes: var passes, CompilePasses: var compilePasses } }:
                    await MatchCodeGeneration(query, languageVersion, ns, passes, compilePasses);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: null, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query }, expected!);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: null, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query, variables }, expected!);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: var operationName, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query, operationName }, expected!);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: var operationName, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query, operationName, variables }, expected!);
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        private async Task MatchCodeGeneration(string query, float languageVersion, string ns, bool passes, bool? compilePasses)
        {
            await Task.Yield();
            var compileResult = CompileManager.CompileString(query, "in-memory.graphql", new GraphQLGenerationOptions { LanguageVersion = languageVersion, Namespace = ns });

            if (passes)
            {
                Assert.Empty(compileResult.Errors);

                if (compilePasses != null)
                {
                    var result = RoslynServices.Compile(compileResult.Code, languageVersion);
                    Assert.Empty(result);
                }
            }
            else
            {
                Assert.NotEmpty(compileResult.Errors);
            }

        }

        private async Task ParseSuccess(IServiceProvider serviceProvider, string query)
        {
            await Task.Yield();
            var astFactory = serviceProvider.GetRequiredService<IAbstractSyntaxTreeGenerator>();
            var document = astFactory.ParseDocument(query);
            Assert.NotNull(document);
        }

        private async Task DetectParsingErrors(IServiceProvider serviceProvider, string query)
        {
            await Task.Yield();
            var astFactory = serviceProvider.GetRequiredService<IAbstractSyntaxTreeGenerator>();
            Assert.Throws<GraphqlParseException>(() => astFactory.ParseDocument(query));
        }

        private async Task Execute(IServiceProvider serviceProvider, string schema, string? setup, object request, string expected)
        {
            var messageResolver = serviceProvider.GetRequiredService<IMessageResolver>();

            if (setup != null)
            {
                using var memoryStream = new MemoryStream();
                await System.Text.Json.JsonSerializer.SerializeAsync(memoryStream, new { query = setup });
                memoryStream.Position = 0;

                using var executor = serviceProvider.GetRequiredService<IGraphQlExecutorFactory>().Create(schema);
                await executor.ExecuteQuery(memoryStream, messageResolver);
            }

            try
            {
                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                using var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestJson));

                var executor = serviceProvider.GetRequiredService<IGraphQlExecutorFactory>().Create(schema);

                var result = await executor.ExecuteQuery(memoryStream, messageResolver);

                var options = JsonOptions.GraphQlJsonSerializerOptions;
                var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions.Setup(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

                Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)), $"Actual: {json}");
            }
            finally
            {
            }
        }

        public void Dispose()
        {
            serviceProvider?.Dispose();
        }
    }
}
