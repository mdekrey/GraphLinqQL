using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;
using Interfaces = GraphLinqQL.HandwrittenSamples.Interfaces;
using Implementations = GraphLinqQL.HandwrittenSamples.Implementations;
using System.Collections.Immutable;
using GraphLinqQL.Execution;
using GraphLinqQL.Stubs;
using GraphLinqQL.Ast;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GraphLinqQL.Resolution;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

namespace GraphLinqQL.Execution
{
    public class GraphQlExecutorShould
    {
        private readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = false
        };

        private class GraphQlExecutionOptions : IGraphQlExecutionOptions
        {
            public Type? Query => typeof(Implementations.QueryContract);

            public Type? Mutation => null;

            public Type? Subscription => null;

            public IReadOnlyList<IGraphQlDirective> Directives { get; } = new IGraphQlDirective[] { new Directives.SkipDirective(), new Directives.IncludeDirective() };

            public IGraphQlTypeResolver TypeResolver { get; } = new Interfaces.TypeResolver();
        }

        private static IGraphQlExecutor CreateExecutor()
        {
            using var serviceProvider = new SimpleServiceProvider();
            return new GraphQlExecutor(serviceProvider, new AbstractSyntaxTreeGenerator(), new GraphQlExecutionOptions(), new LoggerFactory());
        }

        [Fact]
        public async Task BeAbleToRepresentVeryBasicStructures()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  rand
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentUntypedSimpleStructures()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  hero {
    id
    name
  }
  rand
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5,\"hero\":{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentUntypedSimpleListStructures()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
  }
  rand
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"rand\":5}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentNestedStructures()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
    friends {
      id
      name
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToUseStructureFragments()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
fragment HeroPrimary on Hero {
  id
  name
}

{
  heroes {
    ...HeroPrimary
    friends {
      ...HeroPrimary
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentComplexStructures()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
    renown
    faction
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToPassParameters()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
    location
    oldLocation: location(date: ""2008-05-02"")
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToPassParametersWithNonStringTypes()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes(first: 1) {
    id
    name
    renown
    faction
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToPassArguments()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
query Heroes($date: String!) {
  heroes {
    id
    name
    location(date: $date)
  }
}
", arguments: new Dictionary<string, IGraphQlParameterInfo> { { "date", new NewtonsoftJsonParameterInfo("\"2008-05-02\"") } });

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToPassArgumentsWithDefaultValues()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
query Heroes($date: String = ""2019-04-22"", $date2: String = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
", arguments: new Dictionary<string, IGraphQlParameterInfo> { { "date", new NewtonsoftJsonParameterInfo("\"2008-05-02\"") } });

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            //var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

            //Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToUseDirectives()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
    renown @include(if: false)
    faction @skip(if: true)
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToUseInlineFragments()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
    ... {
      renown
      faction
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToUseInlineFragmentsWithTypeConditions()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  heroes {
    id
    name
    ... on Hero {
      renown
      faction
    }
    ... on NonHero {
      crash
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }


        [Fact]
        public async Task BeAbleToUseInlineFragmentsWithTypeConditionsOnUnions()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  characters {
    id
    name
    ... on Hero {
      location
    }
    ... on Villain {
      goal
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"characters\":[{\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\",\"location\":\"Unknown (2019-04-22)\"},{\"id\":\"ASGUARD-3\",\"name\":\"Thor\",\"location\":\"Unknown (2019-04-22)\"},{\"id\":\"AVENGERS-1\",\"name\":\"Captain America\",\"location\":\"Unknown (2019-04-22)\"},{\"id\":\"THANOS\",\"name\":\"Thanos\",\"goal\":\"Snap\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToUseInlineFragmentsWithTypeConditionsOnUnionsWithJoins()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  characters {
    id
    name
    ... on Hero {
      location
      faction
      renown
    }
    ... on Villain {
      goal
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"characters\":[{\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\",\"location\":\"Unknown (2019-04-22)\",\"faction\":\"Guardians of the Galaxy\",\"renown\":5},{\"id\":\"ASGUARD-3\",\"name\":\"Thor\",\"location\":\"Unknown (2019-04-22)\",\"faction\":\"Asgardians\",\"renown\":50},{\"id\":\"AVENGERS-1\",\"name\":\"Captain America\",\"location\":\"Unknown (2019-04-22)\",\"faction\":\"Avengers\",\"renown\":100},{\"id\":\"THANOS\",\"name\":\"Thanos\",\"goal\":\"Snap\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToGetTypenames()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
{
  characters {
    id
    name
    __typename
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"characters\":[{\"name\":\"Starlord\",\"__typename\":\"Hero\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"__typename\":\"Hero\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"__typename\":\"Hero\",\"id\":\"AVENGERS-1\"},{\"name\":\"Thanos\",\"__typename\":\"Villain\",\"id\":\"THANOS\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task HandleNonExistingDocuments()
        {
            using var executor = CreateExecutor();
            var result = await executor.ExecuteAsync(@"
fragment HeroPrimary on Hero {
  id
  name
}");

            Assert.True(result.ErrorDuringParse);
            Assert.Null(result.Data);
            var error = Assert.Single(result.Errors);
            Assert.Equal("noOperationFound", error.ErrorCode);
            Assert.Empty(error.Arguments);
            Assert.Single(error.Locations);
        }
    }

}
