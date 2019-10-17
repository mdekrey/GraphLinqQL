using GraphLinqQL.Stubs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Implementations = GraphLinqQL.HandwrittenSamples.Implementations;
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

namespace GraphLinqQL
{
    public class ResolutionStrategyShould
    {
        private readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = false
        };

        [Fact]
        public async Task BeAbleToHandlePlainObjects()
        {
            // {
            //   hero {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("hero", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("hero")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"hero\":{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToHandlePlainObjectsWithJoin()
        {
            // {
            //   hero {
            //     id
            //     name
            //     renown
            //     faction
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("hero", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("hero")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Add("renown", queryContext.Locations).Add("faction", queryContext.Locations).Build())
                    .Build());

            //var query = from q in new[] { new GraphQlRoot() }.AsQueryable()
            //            let hero = HandwrittenSamples.Domain.Data.heroes.First()
            //            join reputation in HandwrittenSamples.Domain.Data.heroReputation on hero.Id equals reputation.HeroId
            //            select new Dictionary<string, object?>
            //            {
            //                { "id", hero.Id },
            //                { "name", hero.Name },
            //                { "renown", reputation.Renown },
            //                { "faction", reputation.Faction },
            //            };


            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"hero\":{\"faction\":\"Guardians of the Galaxy\",\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\",\"renown\":5}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToHandlePlainObjectsWithFinalizerAndJoin()
        {
            // {
            //   hero {
            //     id
            //     name
            //     renown
            //     faction
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("hero", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroFinalized")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Add("renown", queryContext.Locations).Add("faction", queryContext.Locations).Build())
                    .Build());

            //var query = from q in new[] { new GraphQlRoot() }.AsQueryable()
            //            let hero = HandwrittenSamples.Domain.Data.heroes.First()
            //            join reputation in HandwrittenSamples.Domain.Data.heroReputation on hero.Id equals reputation.HeroId
            //            select new Dictionary<string, object?>
            //            {
            //                { "id", hero.Id },
            //                { "name", hero.Name },
            //                { "renown", reputation.Renown },
            //                { "faction", reputation.Faction },
            //            };


            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"hero\":{\"faction\":\"Guardians of the Galaxy\",\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\",\"renown\":5}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToHandleNulls()
        {
            // {
            //   nohero {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("nohero", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("nohero")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"nohero\":null}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToHandleNullLists()
        {
            // {
            //   nulls {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("nulls", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("nulls")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"nulls\":null}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentUntypedSimpleStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //   }
            //   rand
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                    .Add("rand", queryContext.Locations)
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"id\":\"ASGUARD-3\",\"name\":\"Thor\"},{\"id\":\"AVENGERS-1\",\"name\":\"Captain America\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentSimpleStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //   }
            //   rand
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                    .Add("rand", queryContext.Locations)
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"id\":\"ASGUARD-3\",\"name\":\"Thor\"},{\"id\":\"AVENGERS-1\",\"name\":\"Captain America\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentScalars()
        {
            // {
            //   rand
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("rand", queryContext.Locations)
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentNestedStructures()
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

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp)
                                                                .Add("id", queryContext.Locations)
                                                                .Add("name", queryContext.Locations)
                                                                .Add("friends", queryContext.Locations, hero => ((IGraphQlObjectResult)hero.ResolveQuery("friends")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToDeferNestedStructures()
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

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp)
                                                                .Add("id", queryContext.Locations)
                                                                .Add("name", queryContext.Locations)
                                                                .Add("friends", queryContext.Locations, hero => ((IGraphQlObjectResult)hero.ResolveQuery("friendsDeferred")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToUseTasksToAccessFields()
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

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp)
                                                                .Add("id", queryContext.Locations)
                                                                .Add("name", queryContext.Locations)
                                                                .Add("friends", queryContext.Locations, hero => ((IGraphQlObjectResult)hero.ResolveQuery("friendsTask")).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToRepresentComplexStructures()
        {
            // {
            //   heroes {
            //     id
            //     name
            //     renown
            //     faction
            //   }
            // }

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp)
                                                                .Add("id", queryContext.Locations)
                                                                .Add("name", queryContext.Locations)
                                                                .Add("renown", queryContext.Locations)
                                                                .Add("faction", queryContext.Locations)
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"name\":\"Starlord\",\"renown\":5,\"id\":\"GUARDIANS-1\"},{\"faction\":\"Asgardians\",\"name\":\"Thor\",\"renown\":50,\"id\":\"ASGUARD-3\"},{\"faction\":\"Avengers\",\"name\":\"Captain America\",\"renown\":100,\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToPassParameters()
        {
            // {
            //   heroes {
            //     id
            //     name
            //     location
            //     oldLocation: location(date: "2008-05-02")
            //   }
            // }

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroes")).ResolveComplex(sp)
                                                                .Add("id", queryContext.Locations)
                                                                .Add("name", queryContext.Locations)
                                                                .Add("location", queryContext.Locations)
                                                                .Add("oldLocation", "location", queryContext.Locations, new BasicParameterResolver(new Dictionary<string, IGraphQlParameterInfo> { { "date", new NewtonsoftJsonParameterInfo("\"2008-05-02\"") } }))
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\",\"name\":\"Thor\"},{\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"AVENGERS-1\",\"name\":\"Captain America\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public async Task BeAbleToHandleFirstLevelmplementationJoin()
        {
            // {
            //   heroById(id: "GUARDIANS-1") {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = await sp.GraphQlRootAsync(typeof(Implementations.QueryContract), root =>
                root.Add("heroById", queryContext.Locations, q => ((IGraphQlObjectResult)q.ResolveQuery("heroById", new BasicParameterResolver(new Dictionary<string, IGraphQlParameterInfo>() { { "id", new NewtonsoftJsonParameterInfo("\"GUARDIANS-1\"") } }))).ResolveComplex(sp).Add("id", queryContext.Locations).Add("name", queryContext.Locations).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroById\":{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }



    }
}
