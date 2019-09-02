using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;
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

        [Fact]
        public void BeAbleToRepresentUntypedSimpleStructures()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

            var result = executor.Execute(@"
{
  heroes {
    id
    name
  }
  rand
}
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentNestedStructures()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

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
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[],\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseStructureFragments()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

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
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[],\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentComplexStructures()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

            var result = executor.Execute(@"
{
  heroes {
    id
    name
    renown
    faction
  }
}
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToPassParameters()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

            var result = executor.Execute(@"
{
  heroes {
    id
    name
    location
    oldLocation: location(date: ""2008-05-02"")
  }
}
", ImmutableDictionary<string, object>.Empty);
            
            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToPassArguments()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

            var result = executor.Execute(@"
query Heroes($date: String!) {
  heroes {
    id
    name
    location(date: $date)
  }
}
", new Dictionary<string, object> { { "date", "2008-05-02" } });

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact(Skip = "GraphQL-Parse does not support query default parameters")]
        public void BeAbleToPassArgumentsWithDefaultValues()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

            var result = executor.Execute(@"
query Heroes($date: String = ""2019-04-22"", $date2 = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
", new Dictionary<string, object> { { "date", "2008-05-02" } });

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            //var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"location\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\"}]}";

            //Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseDirectives()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

            var result = executor.Execute(@"
{
  heroes {
    id
    name
    renown @include(if: false)
    faction @skip(if: true)
  }
}
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseInlineFragments()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

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
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact(Skip = "Need to support type conditions")]
        public void BeAbleToUseInlineFragmentsWithTypeConditions()
        {
            var serviceProvider = new SimpleServiceProvider();
            var executor = new GraphQlExecutor<Implementations.Query, Implementations.Query>(serviceProvider);

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
", ImmutableDictionary<string, object>.Empty);

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

    }
}
