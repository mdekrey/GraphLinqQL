using GraphLinqQL.Ast;
using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.TestFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1307 // Specify StringComparison

namespace GraphLinqQL
{
    public sealed class GraphQlWithSqliteShould : IDisposable
    {
        private readonly ServiceProvider serviceProvider;
        private readonly SqliteConnection inMemorySqlite;
        private readonly PassiveReaderCommandInterceptor sqlLogs;

        public GraphQlWithSqliteShould()
        {
            inMemorySqlite = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();

            sqlLogs = new PassiveReaderCommandInterceptor();

            var services = new ServiceCollection();
            services.AddDbContext<StarWarsContext>(options => {
                options.UseSqlite(inMemorySqlite);
                options.AddInterceptors(sqlLogs);
            });
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
                case { Given: { Query: var query, Operation: null, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected, Sqlite: var sqlite } }:
                    await Execute(scope.ServiceProvider, new { query }, expected, sqlite);
                    return;
                case { Given: { Query: var query, Operation: null, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected, Sqlite: var sqlite } }:
                    await Execute(scope.ServiceProvider, new { query, variables }, expected, sqlite);
                    return;
                case { Given: { Query: var query, Operation: var operationName, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected, Sqlite: var sqlite } }:
                    await Execute(scope.ServiceProvider, new { query, operationName }, expected, sqlite);
                    return;
                case { Given: { Query: var query, Operation: var operationName, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected, Sqlite: var sqlite } }:
                    await Execute(scope.ServiceProvider, new { query, operationName, variables }, expected, sqlite);
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

        private async Task Execute(IServiceProvider serviceProvider, object request, string expected, IReadOnlyList<string>? expectedSql)
        {   
            using var memoryStream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(memoryStream, request, request.GetType());
            memoryStream.Position = 0;

            using var executor = serviceProvider.GetRequiredService<IGraphQlExecutorFactory>().Create();
            var messageResolver = serviceProvider.GetRequiredService<IMessageResolver>();

            var contextId = ((IGraphQlExecutionServiceProvider)executor.ServiceProvider).ExecutionServices.GetRequiredService<StarWarsContext>().ContextId.InstanceId;

            try
            {
                var result = await executor.ExecuteQuery(memoryStream, messageResolver);

                var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });

                var allSql = sqlLogs.GetSql(contextId).Select(CleanSql);

                Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)), $"Actual: {json}");

                if (expectedSql != null)
                {
                    expectedSql = expectedSql.Select(CleanSql).ToArray();
                    foreach (var sql in allSql)
                    {
                        Assert.True(expectedSql.Contains(sql), $"Full SQL should be: \n{string.Join("\n", allSql.Select(s => "- " + s))}");
                    }

                    foreach (var sql in expectedSql)
                    {
                        Assert.True(allSql.Contains(sql), $"Full SQL should be: \n{string.Join("\n", allSql.Select(s => "- " + s))}");
                    }
                }
            }
            finally
            {
                var allSql = sqlLogs.GetSql(contextId).Select(CleanSql);

            }
        }

        private string CleanSql(string originalSql)
        {
            return Regex.Replace(originalSql, "\\s+", " ");
        }

        public void Dispose()
        {
            serviceProvider?.Dispose();
            inMemorySqlite?.Dispose();
        }
    }
}
