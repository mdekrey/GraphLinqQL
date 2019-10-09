using GraphLinqQL.Ast;
using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.TestFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

namespace GraphLinqQL
{
    public class GraphQLShould
    {
        private readonly ServiceProvider serviceProvider;

        public GraphQLShould()
        {
            var services = new ServiceCollection();
            services.AddDbContext<StarWarsContext>(options => options.UseInMemoryDatabase("persistent-db-context"));
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
                case { Given: { Query: var query, Variables: null }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, query, expected);
                    return;
                case { Given: { Query: var query, Variables: var variables }, When: { Execute: true }, Then: { MatchResult: var expected } }:
                    await Execute(scope.ServiceProvider, query, variables, expected);
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

        private async Task Execute(IServiceProvider serviceProvider, string query, string expected)
        {
            var request = new { query };
            
            using var memoryStream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(memoryStream, request);
            memoryStream.Position = 0;
            IGraphQlExecutor executor = await ExecuteFromStream(serviceProvider, expected, memoryStream);
        }

        private async Task Execute(IServiceProvider serviceProvider, string query, Dictionary<string, object> variables, string expected)
        {
            var request = new { query, variables };

            using var memoryStream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(memoryStream, request);
            memoryStream.Position = 0;
            IGraphQlExecutor executor = await ExecuteFromStream(serviceProvider, expected, memoryStream);
        }

        private static async Task<IGraphQlExecutor> ExecuteFromStream(IServiceProvider serviceProvider, string expected, MemoryStream memoryStream)
        {
            var executor = serviceProvider.GetRequiredService<IGraphQlExecutorFactory>().Create();
            var messageResolver = serviceProvider.GetRequiredService<IMessageResolver>();

            var result = await executor.ExecuteQuery(memoryStream, messageResolver);

            var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)), $"Actual: {json}");
            return executor;
        }
    }
}
