using GraphQlSchema;
using MorseCode.ITask;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GraphQlResolver.Test
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

        public static class Interfaces
        {
            //type Hero {
            //  id: ID!
            //  name: String!
            //  renown: Float!
            //  faction: String!
            //  friends: [Hero!]!
            //  location(date: String = "2019-04-22"): String!;
            //}
            //type Query {
            //  heroes: [Hero!]!
            //}
            //schema {
            //  query: Query
            //}

            public interface Query : IGraphQlResolvable
            {
                IGraphQlResult<IEnumerable<Hero>> Heroes();
                IGraphQlResult<double> Rand();

                IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
                    name switch
                    {
                        "heroes" => Heroes(),
                        "rand" => Rand(),
                        _ => throw new ArgumentException("Unknown property " + name, nameof(name))
                    };
            }

            public interface Hero : IGraphQlResolvable
            {
                IGraphQlResult<GraphQlId> Id();
                IGraphQlResult<string> Name();
                IGraphQlResult<double> Renown();
                IGraphQlResult<string> Faction();
                IGraphQlResult<IEnumerable<Hero>> Friends();
                IGraphQlResult<string> Location(string date);

                IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
                    name switch
                    {
                        "id" => Id(),
                        "name" => Name(),
                        "renown" => Renown(),
                        "faction" => Faction(),
                        "friends" => Friends(),
                        "location" => Location((string)parameters[0] ?? "2019-04-22"),
                        _ => throw new ArgumentException("Unknown property " + name, nameof(name))
                    };
            }
        }

        public static class Implementations
        {
            public class Query : Interfaces.Query, IGraphQlAccepts<GraphQlRoot>
            {
                public IGraphQlResultFactory<GraphQlRoot> Original { get; set; }

                public IGraphQlResult<IEnumerable<Interfaces.Hero>> Heroes() =>
                    Original.Resolve(root => Domain.heroes).ConvertableList().As<Hero>();

                public IGraphQlResult<double> Rand() =>
                    Original.Resolve(root => 5.0);
            }

            public class Hero : Interfaces.Hero, IGraphQlAccepts<Domain.Hero>
            {
                public IGraphQlResultFactory<Domain.Hero> Original { get; set; }

                private readonly GraphQlJoin<Domain.Hero, Domain.Reputation> reputation =
                    GraphQlJoin.Join<Domain.Hero, Domain.Reputation>((originBase) => from t in originBase
                                                                                     join reputation in Domain.heroReputation on GraphQlJoin.FindOriginal(t).Id equals reputation.HeroId
                                                                                     select GraphQlJoin.BuildPlaceholder(t, reputation));
                private readonly GraphQlJoin<Domain.Hero, IEnumerable<Domain.Hero>> friends =
                    GraphQlJoin.Join<Domain.Hero, IEnumerable<Domain.Hero>>((originBase) => from t in originBase
                                                                                            join friendIds in Domain.friends on GraphQlJoin.FindOriginal(t).Id equals friendIds.Id1
                                                                                            join friend in Domain.heroes on friendIds.Id2 equals friend.Id into friends
                                                                                            select GraphQlJoin.BuildPlaceholder(t, friends));

                public IGraphQlResult<string> Faction() =>
                    Original.Join(reputation).Resolve((hero, reputation) => reputation.Faction);
                public IGraphQlResult<IEnumerable<Interfaces.Hero>> Friends() =>
                    Original.Join(friends).Resolve((hero, friends) => friends).ConvertableList().As<Hero>();
                public IGraphQlResult<GraphQlId> Id() =>
                    Original.Resolve(hero => new GraphQlId(hero.Id));
                public IGraphQlResult<string> Location(string date) =>
                    Original.Resolve(hero => "Unknown"); // TODO - use arguments
                public IGraphQlResult<string> Name() =>
                    Original.Resolve(hero => hero.Name);
                public IGraphQlResult<double> Renown() =>
                    Original.Join(reputation).Resolve((hero, reputation) => (double)reputation.Renown);
            }
        }


        public static class Domain
        {
            public class Hero
            {
                public string Id { get; set; }
                public string Name { get; set; }
            }

            public class Reputation
            {
                public string HeroId { get; set; }
                public float Renown { get; set; }
                public string Faction { get; set; }
            }

            public class Friend
            {
                public string Id1 { get; set; }
                public string Id2 { get; set; }
            }

            public static readonly IReadOnlyList<Hero> heroes = new[]
            {
                new Hero { Id = "GUARDIANS-1", Name = "Starlord" },
                new Hero { Id = "ASGUARD-3", Name = "Thor" },
            };
            public static readonly IReadOnlyList<Reputation> heroReputation = new[]
            {
                new Reputation { HeroId = "GUARDIANS-1", Renown = 5, Faction = "Guardians of the Galaxy" },
                new Reputation { HeroId = "ASGUARD-3", Renown = 50, Faction = "Asgardians" },
            };
            public static readonly IReadOnlyList<Friend> friends = new Friend[]
            {
            };
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
            var query = from q in Resolve.Query<GraphQlRoot>()
                        select new Dictionary<string, object>
                        {
                            { "heroes", from hero in Domain.heroes
                                        select new Dictionary<string, object>
                                        {
                                            { "id", hero.Id },
                                            { "name", hero.Name }
                                        } },
                            { "rand", 5.0 }
                        };
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

            var query = from q in Resolve.Query<GraphQlRoot>()
                        select new
                        {
                            heroes = from hero in Domain.heroes
                                     join friendIds in Domain.friends on hero.Id equals friendIds.Id1
                                     join friend in Domain.heroes on friendIds.Id2 equals friend.Id into friends
                                     select new
                                     {
                                         id = hero.Id,
                                         name = hero.Name,
                                         friends = from friend in friends
                                                   select new
                                                   {
                                                       id = hero.Id,
                                                       name = hero.Name
                                                   }
                                     }
                        };

            var result = new SimpleServiceProvider().GraphQlRoot<Implementations.Query>(root =>
                root.Add("heroes", q => q.Heroes().ResolveComplex()
                                                  .Add("id")
                                                  .Add("name")
                                                  .Add("friends", hero => hero.Friends().ResolveComplex().Add("id").Add("name").Build())
                                                  .Build())
                    .Build());


            // TODO - assert
            Assert.False(true, "Not complete");
            //Assert.True(JToken.DeepEquals(JToken.Parse(json), JToken.Parse(expected)));
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

            var query = from q in Resolve.Query<GraphQlRoot>()
                        select new
                        {
                            heroes = from hero in Resolve.Query<Domain.Hero>()
                                     join reputation in Resolve.Query<KeyValuePair<string, Domain.Reputation>>() on hero.Id equals reputation.Key
                                     select new
                                     {
                                         id = hero.Id,
                                         name = hero.Name,
                                         renown = reputation.Value.Renown,
                                         faction = reputation.Value.Faction
                                     }
                        };

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
