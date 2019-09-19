using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;
using Interfaces = GraphQlResolver.HandwrittenSamples.Interfaces;
using Implementations = GraphQlResolver.HandwrittenSamples.Implementations;
using System.Collections.Immutable;
using GraphQlResolver.Execution;
using GraphQlResolver.Stubs;

namespace GraphQlResolver.Execution
{
    public class GraphQlExecutorShould
    {
        private readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = false
        };

        private class GraphQlExecutionOptions : IGraphQlExecutionOptions
        {
            public Type? Query => typeof(Implementations.Query);

            public Type? Mutation => null;

            public Type? Subscription => null;

            public IReadOnlyList<IGraphQlDirective> Directives { get; } = new IGraphQlDirective[] { new Directives.SkipDirective(), new Directives.IncludeDirective() };

            public IGraphQlTypeResolver TypeResolver { get; } = new Interfaces.TypeResolver();
        }

        private static IGraphQlExecutor CreateExecutor()
        {
            var serviceProvider = new SimpleServiceProvider();
            return new GraphQlExecutor(serviceProvider, new GraphQlExecutionOptions());
        }

        [Fact]
        public void BeAbleToRepresentUntypedSimpleStructures()
        {
            var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  hero {
    id
    name
  }
  rand
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"rand\":5,\"hero\":{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentUntypedSimpleListStructures()
        {
            var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
  }
  rand
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"rand\":5}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentNestedStructures()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseStructureFragments()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentComplexStructures()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToPassParameters()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToPassParametersWithNonStringTypes()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToPassArguments()
        {
            var executor = CreateExecutor();
            var result = executor.Execute(@"
query Heroes($date: String!) {
  heroes {
    id
    name
    location(date: $date)
  }
}
", new Dictionary<string, string> { { "date", "\"2008-05-02\"" } });

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact(Skip = "GraphQL-Parse does not support query default parameters")]
        public void BeAbleToPassArgumentsWithDefaultValues()
        {
            var executor = CreateExecutor();
            var result = executor.Execute(@"
query Heroes($date: String = ""2019-04-22"", $date2 = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
", new Dictionary<string, string> { { "date", "\"2008-05-02\"" } });

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            //var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

            //Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseDirectives()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseInlineFragments()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseInlineFragmentsWithTypeConditions()
        {
            var executor = CreateExecutor();
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

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }


        [Fact]
        public void BeAbleToUseInlineFragmentsWithTypeConditionsOnUnions()
        {
            var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  characters {
    id
    name
    ... on Hero {
      renown
      faction
    }
    ... on Villain {
      goal
    }
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"characters\":[{\"name\":\"Starlord\",\"renown\":5,\"faction\":\"Guardians of the Galaxy\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"renown\":50,\"faction\":\"Asgardians\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"renown\":100,\"faction\":\"Avengers\",\"id\":\"AVENGERS-1\"},{\"name\":\"Thanos\",\"goal\":\"Snap\",\"id\":\"THANOS\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToGetTypenames()
        {
            var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  characters {
    id
    name
    __typename
  }
}
");

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"characters\":[{\"name\":\"Starlord\",\"__typename\":\"Hero\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"__typename\":\"Hero\",\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"__typename\":\"Hero\",\"id\":\"AVENGERS-1\"},{\"name\":\"Thanos\",\"__typename\":\"Villain\",\"id\":\"THANOS\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }
    }

}
