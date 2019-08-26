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
                IQueryable<IReadOnlyList<Hero>> Heroes();

                IQueryable IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
                    name switch
                    {
                        "heroes" => Heroes(),
                        _ => throw new ArgumentException("Unknown property " + name, nameof(name))
                    };
            }

            public interface Hero : IGraphQlResolvable
            {
                IQueryable<GraphQlId> Id();
                IQueryable<string> Name();
                IQueryable<double> Renown();
                IQueryable<string> Faction();
                IQueryable<IReadOnlyList<Hero>> Friends();
                IQueryable<string> Location(string date);

                IQueryable IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
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
            public class Query : Interfaces.Query
            {
                public IQueryable<IReadOnlyList<Interfaces.Hero>> Heroes()
                {
                    return Resolve.From<AllHeroesFactory, IReadOnlyList<Domain.Hero>>().AsGraphQl<Domain.Hero, Hero>();
                }
            }

            public class Hero : Interfaces.Hero, IGraphQlAccepts<Domain.Hero>
            {
                public IQueryable<Domain.Hero> Original { get; set; }

                public IQueryable<string> Faction() => Original
                    .Select(hero => hero.Id)
                    .Via<HeroReputationFactory, Domain.Reputation, string>()
                    .Select(rep => rep.Faction);

                public IQueryable<IReadOnlyList<Interfaces.Hero>> Friends() =>
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
                    return Task.FromResult(Domain.heroReputation[input]);
                }

                public Task<Func<string, Domain.Reputation>> ResolveMany(IReadOnlyList<string> input)
                {
                    return Task.FromResult<Func<string, Domain.Reputation>>(id => Domain.heroReputation[id]);
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
                public float Renown { get; set; }
                public string Faction { get; set; }
            }

            public static readonly IReadOnlyList<Hero> heroes = new[]
            {
                new Hero { Id = "GUARDIANS-1", Name = "Starlord" },
                new Hero { Id = "ASGUARD-3", Name = "Thor" },
            };
            public static readonly IReadOnlyDictionary<string, Reputation> heroReputation = new Dictionary<string, Reputation>
            {
                { "GUARDIANS-1", new Reputation { Renown = 5, Faction = "Guardians of the Galaxy" } },
                { "ASGUARD-3", new Reputation { Renown = 50, Faction = "Asgardians" } },
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
            var strategy = Resolve.Combine(new CombineOptions
            {
                { "heroes", Resolve.FromMany<Implementations.AllHeroesFactory, Domain.Hero>()
                                 .Then(baseStrategy => Resolve.Combine(new CombineOptions
                                                       {
                                                           { "id", baseStrategy.Select(hero => hero.Id) },
                                                           { "name", baseStrategy.Select(hero => hero.Name) },
                                                       })) 
                }
            });
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
            var strategy = Resolve.Combine(new CombineOptions
            {
                { "heroes", Resolve.FromMany<Implementations.AllHeroesFactory, Domain.Hero>()
                                 .Then(baseStrategy => Resolve.Combine(new CombineOptions
                                                       {
                                                           { "id", baseStrategy.Select(hero => hero.Id) },
                                                           { "name", baseStrategy.Select(hero => hero.Name) },
                                                           { "friends", Resolve.ViaMany<Implementations.HeroFriendsFactory, Domain.Hero, string>(baseStrategy.Select(hero => hero.Id))
                                                                               .Then(baseStrategy => Resolve.Combine(new CombineOptions
                                                                                                     {
                                                                                                         { "id", baseStrategy.Select(hero => hero.Id) },
                                                                                                         { "name", baseStrategy.Select(hero => hero.Name) },
                                                                                                     }))
                                                           },
                                                       }))
                }
            });
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
            var strategy = Resolve.Combine(new CombineOptions
            {
                { "heroes", Resolve.FromMany<Implementations.AllHeroesFactory, Domain.Hero>()
                                 .Then(baseStrategy =>
                                 {
                                     var reputation = baseStrategy.Select(hero => hero.Id).Via<Implementations.HeroReputationFactory, Domain.Reputation, string>();
                                     return Resolve.Combine(new CombineOptions
                                     {
                                        { "id", baseStrategy.Select(hero => hero.Id) },
                                        { "name", baseStrategy.Select(hero => hero.Name) },
                                        { "renown", reputation.Select(rep => rep.Renown) },
                                        { "faction", reputation.Select(rep => rep.Faction) },
                                     });
                                 })
                }
            });
        }

    }
}
