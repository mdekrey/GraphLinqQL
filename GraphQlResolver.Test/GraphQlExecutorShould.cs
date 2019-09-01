using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Implementations = GraphQlResolver.HandwrittenSamples.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace GraphQlResolver
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

    }
}
