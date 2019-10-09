using GraphLinqQL.Stubs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using Implementations = GraphLinqQL.HandwrittenSamples.Implementations;

namespace GraphLinqQL
{
    public class ResolutionStrategyShould
    {
        private readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions()
        {
            WriteIndented = false
        };

        [Fact]
        public void BeAbleToHandlePlainObjects()
        {
            // {
            //   hero {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("hero", queryContext, q => q.ResolveQuery(queryContext, "hero").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"hero\":{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToHandlePlainObjectsWithJoin()
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("hero", queryContext, q => q.ResolveQuery(queryContext, "hero").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Add("renown", queryContext).Add("faction", queryContext).Build())
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
        public void BeAbleToHandlePlainObjectsWithFinalizerAndJoin()
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("hero", queryContext, q => q.ResolveQuery(queryContext, "heroFinalized").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Add("renown", queryContext).Add("faction", queryContext).Build())
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
        public void BeAbleToHandleNulls()
        {
            // {
            //   nohero {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("nohero", queryContext, q => q.ResolveQuery(queryContext, "nohero").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"nohero\":null}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToHandleNullLists()
        {
            // {
            //   nulls {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("nulls", queryContext, q => q.ResolveQuery(queryContext, "nulls").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"nulls\":null}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
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
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                    .Add("rand", queryContext)
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"id\":\"ASGUARD-3\",\"name\":\"Thor\"},{\"id\":\"AVENGERS-1\",\"name\":\"Captain America\"}]}";

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
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                    .Add("rand", queryContext)
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5,\"heroes\":[{\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"id\":\"ASGUARD-3\",\"name\":\"Thor\"},{\"id\":\"AVENGERS-1\",\"name\":\"Captain America\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToRepresentScalars()
        {
            // {
            //   rand
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("rand", queryContext)
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"rand\":5}";

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

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext)
                                                                .Add("id", queryContext)
                                                                .Add("name", queryContext)
                                                                .Add("friends", queryContext, hero => hero.ResolveQuery(queryContext, "friends").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToDeferNestedStructures()
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext)
                                                                .Add("id", queryContext)
                                                                .Add("name", queryContext)
                                                                .Add("friends", queryContext, hero => hero.ResolveQuery(queryContext, "friendsDeferred").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToUseTasksToAccessFields()
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext)
                                                                .Add("id", queryContext)
                                                                .Add("name", queryContext)
                                                                .Add("friends", queryContext, hero => hero.ResolveQuery(queryContext, "friendsTask").ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"name\":\"Starlord\",\"friends\":[],\"id\":\"GUARDIANS-1\"},{\"name\":\"Thor\",\"friends\":[{\"name\":\"Captain America\",\"id\":\"AVENGERS-1\"}],\"id\":\"ASGUARD-3\"},{\"name\":\"Captain America\",\"friends\":[{\"name\":\"Thor\",\"id\":\"ASGUARD-3\"}],\"id\":\"AVENGERS-1\"}]}";

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

            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext)
                                                                .Add("id", queryContext)
                                                                .Add("name", queryContext)
                                                                .Add("renown", queryContext)
                                                                .Add("faction", queryContext)
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"faction\":\"Guardians of the Galaxy\",\"name\":\"Starlord\",\"renown\":5,\"id\":\"GUARDIANS-1\"},{\"faction\":\"Asgardians\",\"name\":\"Thor\",\"renown\":50,\"id\":\"ASGUARD-3\"},{\"faction\":\"Avengers\",\"name\":\"Captain America\",\"renown\":100,\"id\":\"AVENGERS-1\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToPassParameters()
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", queryContext, q => q.ResolveQuery(queryContext, "heroes").ResolveComplex(sp, queryContext)
                                                                .Add("id", queryContext)
                                                                .Add("name", queryContext)
                                                                .Add("location", queryContext)
                                                                .Add("oldLocation", "location", queryContext, new Dictionary<string, IGraphQlParameterInfo> { { "date", new NewtonsoftJsonParameterInfo("\"2008-05-02\"") } })
                                                                .Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroes\":[{\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"GUARDIANS-1\",\"name\":\"Starlord\"},{\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"ASGUARD-3\",\"name\":\"Thor\"},{\"location\":\"Unknown (2019-04-22)\",\"oldLocation\":\"Unknown (2008-05-02)\",\"id\":\"AVENGERS-1\",\"name\":\"Captain America\"}]}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }

        [Fact]
        public void BeAbleToHandleFirstLevelmplementationJoin()
        {
            // {
            //   heroById(id: "GUARDIANS-1") {
            //     id
            //     name
            //   }
            // }
            using var sp = new SimpleServiceProvider();
            var queryContext = FieldContext.Empty;
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroById", queryContext, q => q.ResolveQuery("heroById", queryContext, new BasicParameterResolver(new Dictionary<string, IGraphQlParameterInfo>() { { "id", new NewtonsoftJsonParameterInfo("\"GUARDIANS-1\"") } })).ResolveComplex(sp, queryContext).Add("id", queryContext).Add("name", queryContext).Build())
                    .Build());

            var json = System.Text.Json.JsonSerializer.Serialize(result.Data, JsonOptions);
            var expected = "{\"heroById\":{\"name\":\"Starlord\",\"id\":\"GUARDIANS-1\"}}";

            Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
        }



    }
}
