using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;
using Interfaces = GraphQlResolver.HandwrittenSamples.Interfaces;
using Implementations = GraphQlResolver.HandwrittenSamples.Implementations;
using System.Collections.Immutable;
using GraphQlResolver.Execution;

namespace GraphQlResolver.Execution
{
    public class GraphQlExecutorShould
    {
        private readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = false
        };

        private class SimpleServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return Activator.CreateInstance(serviceType) ?? throw new NotSupportedException();
            }
        }

        private static GraphQlExecutor<Implementations.Query, Implementations.Query, Interfaces.TypeResolver> CreateExecutor()
        {
            var serviceProvider = new SimpleServiceProvider();
            return new GraphQlExecutor<Implementations.Query, Implementations.Query, Interfaces.TypeResolver>(serviceProvider, new Interfaces.TypeResolver());
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
", _ => ImmutableDictionary<string, object?>.Empty);

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}]}";

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[],\"id\":\"ASGUARD-3\"}]}";

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[],\"id\":\"ASGUARD-3\"}]}";

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

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
", _ => ImmutableDictionary<string, object?>.Empty);
            
            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

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
", _ => new Dictionary<string, object?> { { "date", "2008-05-02" } });

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

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
", _ => new Dictionary<string, object?> { { "date", "2008-05-02" } });

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}]}";

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

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
", _ => ImmutableDictionary<string, object?>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

    }
}
