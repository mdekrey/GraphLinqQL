using GraphQlSchema;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GraphQlResolver.Test
{
    public class ResolutionStrategyShould
    {
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
                IGraphQlListResolver<Hero> Heroes();
                IQueryable<double> Rand();

                object IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
                    name switch
                    {
                        "heroes" => Heroes(),
                        "rand" => Rand(),
                        _ => throw new ArgumentException("Unknown property " + name, nameof(name))
                    };
            }

            public interface Hero : IGraphQlResolvable
            {
                IQueryable<GraphQlId> Id();
                IQueryable<string> Name();
                IQueryable<double> Renown();
                IQueryable<string> Faction();
                IGraphQlListResolver<Hero> Friends();
                IQueryable<string> Location(string date);

                object IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
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
                public IQueryable<GraphQlRoot> Original { get; set; }
                IQueryable IGraphQlAccepts.Original { set => this.Original = (IQueryable<GraphQlRoot>)value; }

                public IQueryable<double> Rand() => from e in Original select 5.0;

                public IGraphQlListResolver<Interfaces.Hero> Heroes() => Original.ViaMany<AllHeroesFactory, Hero, Domain.Hero>();

            }

            public class Hero : Interfaces.Hero, IGraphQlAccepts<Domain.Hero>
            {
                public IQueryable<Domain.Hero> Original { get; set; }
                IQueryable IGraphQlAccepts.Original { set => this.Original = (IQueryable<Domain.Hero>)value; }

                public IQueryable<string> Faction() => Original
                    .Select(hero => hero.Id)
                    .Via<HeroReputationFactory, Domain.Reputation, string>()
                    .Select(rep => rep.Faction);

                public IGraphQlListResolver<Interfaces.Hero> Friends() =>
                    Original.Select(hero => hero.Id)
                            .ViaMany<HeroFriendsFactory, Hero, string, Domain.Hero>();

                public IQueryable<GraphQlId> Id() => Original.Select(hero => new GraphQlId(hero.Id));

                public IQueryable<string> Name() => Original.Select(hero => hero.Name);

                public IQueryable<double> Renown() => Original.Select(hero => hero.Id).Via<HeroReputationFactory, Domain.Reputation, string>().Select(rep => (double)rep.Renown);

                public IQueryable<string> Location(string date) =>
                    from original in Original
                    select "Unknown";

            }



            public class AllHeroesFactory : IResolutionFactory<IReadOnlyList<Domain.Hero>>
            {
                public Task<IReadOnlyList<Domain.Hero>> Resolve()
                {
                    return Task.FromResult(Domain.heroes);
                }
            }

            public class HeroFriendsFactory : IResolutionFactory<IReadOnlyList<Domain.Hero>, string>
            {
                public Task<IReadOnlyList<Domain.Hero>> Resolve(string input)
                {
                    return Task.FromResult((IReadOnlyList<Domain.Hero>)Array.Empty<Domain.Hero>());
                }

                public Task<Func<string, IReadOnlyList<Domain.Hero>>> ResolveMany(IReadOnlyList<string> input)
                {
                    return Task.FromResult<Func<string, IReadOnlyList<Domain.Hero>>>(id => Array.Empty<Domain.Hero>());
                }
            }

            public class HeroReputationFactory : IResolutionFactory<Domain.Reputation, string>
            {
                public Task<Domain.Reputation> Resolve(string input)
                {
                    return Task.FromResult(Domain.heroReputation.First(r => r.HeroId == input));
                }

                public Task<Func<string, Domain.Reputation>> ResolveMany(IReadOnlyList<string> input)
                {
                    return Task.FromResult<Func<string, Domain.Reputation>>(id => Domain.heroReputation.First(r => r.HeroId == id));
                }
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
            public static readonly IReadOnlyList<Reputation> heroReputation = new []
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
            // }
            var root = Resolve.Root<Implementations.Query>();
            var final = root.ResolveComplex()
                .Add("heroes", q => q.Heroes().ResolveComplex().Add("id").Add("name").Build())
                .Add("rand")
                .Build();
            var query = from q in Resolve.Query<GraphQlRoot>()
                        select new
                        {
                            heroes = from hero in Domain.heroes
                                     select new
                                     {
                                         id = hero.Id,
                                         name = hero.Name
                                     }
                        };

            var strategy = new Dictionary<string, IQueryable>
            {
                { "heroes",  Resolve.FromMany<Implementations.AllHeroesFactory, Domain.Hero>()
                                 .Combine(new CombineOptions<Domain.Hero>
                                                       {
                                                           { "id", baseStrategy => baseStrategy.Select(hero => hero.Id) },
                                                           { "name", baseStrategy => baseStrategy.Select(hero => hero.Name) },
                                                       })
                }
            };
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

            var root = Resolve.Root<Implementations.Query>();
            var final = root.ResolveComplex()
                .Add("heroes", q => q.Heroes().ResolveComplex()
                                              .Add("id")
                                              .Add("name")
                                              .Add("friends", hero => hero.Friends().ResolveComplex().Add("id").Add("name").Build())
                                              .Build())
                .Build();


            var query = from q in Resolve.Query<GraphQlRoot>()
                        select new
                        {
                            heroes = from hero in Domain.heroes
                                     join friendIds in Domain.friends on hero.Id equals friendIds.Id1 join friend in Domain.heroes on friendIds.Id2 equals friend.Id into friends
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

            var strategy = new Dictionary<string, IQueryable>
            {
                { "heroes", Resolve.FromMany<Implementations.AllHeroesFactory, Domain.Hero>()
                                   .Combine(new CombineOptions<Domain.Hero>
                                                       {
                                                           { "id", baseStrategy => baseStrategy.Select(hero => hero.Id) },
                                                           { "name", baseStrategy => baseStrategy.Select(hero => hero.Name) },
                                                           { "friends", baseStrategy => Resolve.ViaMany<Implementations.HeroFriendsFactory, Domain.Hero, string>(baseStrategy.Select(hero => hero.Id))
                                                                               .Combine(new CombineOptions<Domain.Hero>
                                                                                                     {
                                                                                                         { "id", baseStrategy => baseStrategy.Select(hero => hero.Id) },
                                                                                                         { "name", baseStrategy => baseStrategy.Select(hero => hero.Name) },
                                                                                                     })
                                                           },
                                                       })
                }
            };
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

            var root = Resolve.Root<Implementations.Query>();
            var final = root.ResolveComplex()
                .Add("heroes", q => q.Heroes().ResolveComplex()
                                              .Add("id")
                                              .Add("name")
                                              .Add("renown")
                                              .Add("faction")
                                              .Build())
                .Build();

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

            var strategy = new Dictionary<string, IQueryable>
            {
                { "heroes", Resolve.FromMany<Implementations.AllHeroesFactory, Domain.Hero>()
                                   .Combine(new CombineOptions<Domain.Hero>
                                     {
                                        { "id", baseStrategy => baseStrategy.Select(hero => hero.Id) },
                                        { "name", baseStrategy => baseStrategy.Select(hero => hero.Name) },
                                        { "renown", baseStrategy => baseStrategy
                                            .Select(hero => hero.Id).Via<Implementations.HeroReputationFactory, Domain.Reputation, string>()
                                            .Select(rep => rep.Renown) },
                                        { "faction", baseStrategy => baseStrategy
                                            .Select(hero => hero.Id).Via<Implementations.HeroReputationFactory, Domain.Reputation, string>()
                                            .Select(rep => rep.Faction) },
                                     })
                }
            };
        }

    }
}
