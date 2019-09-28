using GraphLinqQL.Stubs;
using Newtonsoft.Json.Linq;
using Snapper;
using Snapper.Attributes;
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("hero", q => q.ResolveQuery("hero").ResolveComplex(sp).Add("id").Add("name").Build())
                    .Build());
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("hero", q => q.ResolveQuery("hero").ResolveComplex(sp).Add("id").Add("name").Add("renown").Add("faction").Build())
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

            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("hero", q => q.ResolveQuery("heroFinalized").ResolveComplex(sp).Add("id").Add("name").Add("renown").Add("faction").Build())
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
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("nohero", q => q.ResolveQuery("nohero").ResolveComplex(sp).Add("id").Add("name").Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("nulls", q => q.ResolveQuery("nulls").ResolveComplex(sp).Add("id").Add("name").Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex(sp).Add("id").Add("name").Build())
                    .Add("rand")
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex(sp).Add("id").Add("name").Build())
                    .Add("rand")
                    .Build());
            
            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToRepresentScalars()
        {
            // {
            //   rand
            // }
            using var sp = new SimpleServiceProvider();
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("rand")
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex(sp)
                                                                .Add("id")
                                                                .Add("name")
                                                                .Add("friends", hero => hero.ResolveQuery("friends").ResolveComplex(sp).Add("id").Add("name").Build())
                                                                .Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex(sp)
                                                                .Add("id")
                                                                .Add("name")
                                                                .Add("friends", hero => hero.ResolveQuery("friendsDeferred").ResolveComplex(sp).Add("id").Add("name").Build())
                                                                .Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex(sp)
                                                                .Add("id")
                                                                .Add("name")
                                                                .Add("renown")
                                                                .Add("faction")
                                                                .Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroes", q => q.ResolveQuery("heroes").ResolveComplex(sp)
                                                                .Add("id")
                                                                .Add("name")
                                                                .Add("location")
                                                                .Add("oldLocation", "location", new Dictionary<string, string> { { "date", "\"2008-05-02\"" } })
                                                                .Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
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
            var result = sp.GraphQlRoot(typeof(Implementations.QueryContract), root =>
                root.Add("heroById", q => q.ResolveQuery("heroById", new BasicParameterResolver(new Dictionary<string, string>() { { "id", "\"GUARDIANS-1\"" } })).ResolveComplex(sp).Add("id").Add("name").Build())
                    .Build());
            
            result.ShouldMatchSnapshot();
        }



    }
}
