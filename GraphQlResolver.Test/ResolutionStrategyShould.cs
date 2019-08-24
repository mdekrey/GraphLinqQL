using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GraphQlResolver.Test
{
    public class ResolutionStrategyShould
    {
        public class AllHeroesFactory : IResolutionFactory<Hero>
        {
        }

        public class HeroFriendsFactory : IResolutionFactory<IList<Hero>, string>
        {
        }

        public class HeroReputationFactory : IResolutionFactory<Reputation, string>
        {
        }

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

        [Fact]
        public void BeAbleToRepresentSimpleStructures()
        {
            // {
            //   hero {
            //     id
            //     name
            //   }
            // }
            var strategy = Resolve.Combine(new CombineOptions
            {
                { "hero", Resolve.From<AllHeroesFactory, Hero>()
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
            //   hero {
            //     id
            //     name
            //     friends {
            //       id
            //       name
            //     }
            //   }
            // }
            var baseStrategy = Resolve.From<AllHeroesFactory, Hero>();
            var strategy = Resolve.Combine(new CombineOptions
            {
                { "hero", Resolve.From<AllHeroesFactory, Hero>()
                                 .Then(baseStrategy => Resolve.Combine(new CombineOptions
                                                       {
                                                           { "id", baseStrategy.Select(hero => hero.Id) },
                                                           { "name", baseStrategy.Select(hero => hero.Name) },
                                                           { "friends", Resolve.FromMany<HeroFriendsFactory, Hero, string>(baseStrategy.Select(hero => hero.Id))
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
            //   hero {
            //     id
            //     name
            //     renown
            //     faction
            //   }
            // }
            var strategy = Resolve.Combine(new CombineOptions
            {
                { "hero", Resolve.From<AllHeroesFactory, Hero>()
                                 .Then(baseStrategy =>
                                 {
                                     var reputation = Resolve.From<HeroReputationFactory, Reputation, string>(baseStrategy.Select(hero => hero.Id));
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
