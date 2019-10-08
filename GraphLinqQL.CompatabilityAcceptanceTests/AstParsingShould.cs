using GraphLinqQL.Ast;
using GraphLinqQL.CompatabilityAcceptanceTests;
using GraphLinqQL.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace GraphLinqQL
{
    public class AstParsingShould
    {

        [Theory]
        [YamlScenarios("scenarios.parsing.")]
        public void Handle(ScenarioDataNode<ParsingGiven, ParsingWhen, ParsingThen> scenario)
        {
            switch (scenario)
            {
                case { Given: var given, When: { Parse: true }, Then: { Passes: true } }:
                    ParseWithoutError(given);
                    return;
                case { Given: var given, When: { Parse: true }, Then: { SyntaxError: true } }:
                    DetectParsingErrors(given);
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        private void ParseWithoutError(ParsingGiven given)
        {
            var astFactory = new AbstractSyntaxTreeGenerator();
            var document = astFactory.ParseDocument(given.Query);
            Assert.NotNull(document);
        }

        private void DetectParsingErrors(ParsingGiven given)
        {
            var astFactory = new AbstractSyntaxTreeGenerator();
            Assert.Throws<GraphqlParseException>(() => astFactory.ParseDocument(given.Query));
        }

        [Fact]
        public void BeAbleToGetData()
        {
            var attr = new YamlScenariosAttribute("scenarios.parsing.");
            var data = attr.GetData(typeof(AstParsingShould).GetMethod(nameof(Handle))!);
            Assert.All(data, datum =>
            {
                var scenario = Assert.Single(datum);
                Assert.IsType<ScenarioDataNode<ParsingGiven, ParsingWhen, ParsingThen>>(scenario);
            });
            var scenarios = data.Select(datum => datum.Single()).Cast<ScenarioDataNode<ParsingGiven, ParsingWhen, ParsingThen>>().ToArray();

            Assert.NotEmpty(scenarios);
            Assert.All(scenarios, scenario => {
                Assert.NotNull(scenario.Name);
                Assert.NotNull(scenario.Given);
                Assert.NotNull(scenario.When);
                Assert.NotNull(scenario.Then);
            });

            // Ensure we have at least one of each of these, or we parsed the yaml wrong
            Assert.Contains(scenarios, scenario => scenario.When.Parse);
            Assert.Contains(scenarios, scenario => scenario.Then.Passes);
            Assert.Contains(scenarios, scenario => scenario.Then.SyntaxError);
        }
    }
}
