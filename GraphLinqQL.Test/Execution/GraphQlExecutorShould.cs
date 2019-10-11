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
        public void BeAbleToRepresentVeryBasicStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  rand
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentUntypedSimpleStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToRepresentUntypedSimpleListStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToRepresentNestedStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToUseStructureFragments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToRepresentComplexStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToPassParameters()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToPassParametersWithNonStringTypes()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToPassArguments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToPassArgumentsWithDefaultValues()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToUseDirectives()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToUseInlineFragments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToUseInlineFragmentsWithTypeConditions()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToUseInlineFragmentsWithTypeConditionsOnUnions()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void BeAbleToGetTypenames()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
        public void HandleNonExistingDocuments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
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
