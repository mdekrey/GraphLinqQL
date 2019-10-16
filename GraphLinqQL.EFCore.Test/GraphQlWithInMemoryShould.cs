﻿using GraphLinqQL.Ast;
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
            services.AddDbContext<StarWarsContext>(options => options.UseInMemoryDatabase(nameof(GraphQlWithInMemoryShould)));
            services.AddGraphQl<StarWars.Interfaces.TypeResolver>("star-wars", typeof(StarWars.Implementations.Query), options =>
            {
                options.Mutation = typeof(StarWars.Implementations.Mutation);
                options.AddIntrospection();
            });
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
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: null, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query }, expected);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: null, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query, variables }, expected);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: var operationName, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query, operationName }, expected);
                    return;
                case { Given: { Schema: var schema, SetupQuery: var setup, Query: var query, Operation: var operationName, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, schema, setup, new { query, operationName, variables }, expected);
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
                using var memoryStream = new MemoryStream();
                await System.Text.Json.JsonSerializer.SerializeAsync(memoryStream, request, request.GetType());
                memoryStream.Position = 0;

                var executor = serviceProvider.GetRequiredService<IGraphQlExecutorFactory>().Create(schema);

                var result = await executor.ExecuteQuery(memoryStream, messageResolver);

                var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });

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
