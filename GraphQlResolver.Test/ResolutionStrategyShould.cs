using Newtonsoft.Json.Linq;
using System;
using Xunit;
using Implementations = GraphQlResolver.HandwrittenSamples.Implementations;

namespace GraphQlResolver
{
    public class ResolutionStrategyShould
    {
        private readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = false
        };

        private class SimpleServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return Activator.CreateInstance(serviceType);
            }
        }

        [Fact]
        public void BeAbleToRepresentUntypedSimpleStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //   }
            //   rand
            // }
            var result = new SimpleServiceProvider().GraphQlRoot(typeof(Implementations.Query), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex().Add("id").Add("name").Build())
                    .Add("rand")
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentSimpleStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //   }
            //   rand
            // }
            var result = new SimpleServiceProvider().GraphQlRoot<Implementations.Query>(root =>
                root.Add("heroes", q => q.Heroes().ResolveComplex().Add("id").Add("name").Build())
                    .Add("rand")
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentNestedStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //     friends {
            //       id
            //       name
            //     }
            //   }
            // }

            var result = new SimpleServiceProvider().GraphQlRoot<Implementations.Query>(root =>
                root.Add("heroes", q => q.Heroes().ResolveComplex()
                                                  .Add("id")
                                                  .Add("name")
                                                  .Add("friends", hero => hero.Friends().ResolveComplex().Add("id").Add("name").Build())
                                                  .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[],\"id\":\"ASGUARD-3\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentComplexStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //     renown
            //     faction
            //   }
            // }

            var result = new SimpleServiceProvider().GraphQlRoot<Implementations.Query>(root =>
                root.Add("heroes", q => q.Heroes().ResolveComplex()
                                                  .Add("id")
                                                  .Add("name")
                                                  .Add("renown")
                                                  .Add("faction")
                                                  .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"renown\":5,\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"faction\":\"Asgardians\",\"renown\":50,\"id\":\"ASGUARD-3\",\"name\":\"Thor\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

    }
}
