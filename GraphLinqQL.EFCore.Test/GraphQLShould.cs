using GraphLinqQL.Ast;
using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.TestFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace GraphLinqQL
{
    public sealed class GraphQLShould : IDisposable
    {
        private readonly ServiceProvider serviceProvider;
        private readonly SqliteConnection inMemorySqlite;

        public GraphQLShould()
        {
            inMemorySqlite = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();

            var services = new ServiceCollection();
            services.AddDbContext<StarWarsContext>(options => options.UseSqlite(inMemorySqlite));
            services.AddGraphQl<Sample.Interfaces.TypeResolver>(typeof(Sample.Implementations.Query), options => options.AddIntrospection());
            services.AddLogging();
            serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<StarWarsContext>().Database.EnsureCreated();
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
                case { Given: { Query: var query, Operation: null, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, new { query }, expected);
                    return;
                case { Given: { Query: var query, Operation: null, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, new { query, variables }, expected);
                    return;
                case { Given: { Query: var query, Operation: var operationName, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, new { query, operationName }, expected);
                    return;
                case { Given: { Query: var query, Operation: var operationName, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, new { query, operationName, variables }, expected);
                    return;
                default:
                    throw new NotSupportedException();
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

        private async Task Execute(IServiceProvider serviceProvider, object request, string expected)
        {   
            using var memoryStream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(memoryStream, request, request.GetType());
            memoryStream.Position = 0;

            var executor = serviceProvider.GetRequiredService<IGraphQlExecutorFactory>().Create();
            var messageResolver = serviceProvider.GetRequiredService<IMessageResolver>();

            var result = await executor.ExecuteQuery(memoryStream, messageResolver);

            var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)), $"Actual: {json}");
        }

        public void Dispose()
        {
            serviceProvider?.Dispose();
            inMemorySqlite?.Dispose();
        }
    }
}
