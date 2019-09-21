﻿using GraphLinqQL.CommonTypes;
using GraphLinqQL.HandwrittenSamples.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.HandwrittenSamples.Implementations
{
    public class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlResult<IEnumerable<Interfaces.Hero>> Heroes(int? first) =>
            first == null 
                ? Original.Resolve(root => Domain.Data.heroes).List(item => item.As<Hero>())
                : Original.Resolve(root => Domain.Data.heroes.Take(first.Value)).List(item => item.As<Hero>());

        public override IGraphQlResult<IEnumerable<Interfaces.Hero>?> Nulls() =>
            Original.Resolve(root => (IEnumerable<Domain.Hero>?)null).Nullable(nullable => nullable.List(item => item.As<Hero>()));

        public override IGraphQlResult<Interfaces.Hero?> NoHero() =>
            Original.Resolve(root => (Domain.Hero?)null).Nullable(nullable => nullable.As<Hero>());

        public override IGraphQlResult<Interfaces.Hero> Hero() =>
            Original.Resolve(root => Domain.Data.heroes.First()).As<Hero>();

        public override IGraphQlResult<Interfaces.Hero> HeroFinalized() =>
            Original.Resolve(root => Domain.Data.heroes).List(item => item.As<Hero>()).Only();

        public override IGraphQlResult<Interfaces.Hero> HeroById(string id) =>
            Original.Resolve(root => id).As<HeroById>();

        public override IGraphQlResult<double> Rand() =>
            Original.Resolve(root => 5.0);

        public override IGraphQlResult<IEnumerable?> Characters() =>
            Original.Resolve(root => Domain.Data.heroes).List(item => item.As<Hero>())
            .Union<IEnumerable<IGraphQlResolvable>?>(Original.Resolve(_ => Domain.Data.villains).List(item => item.As<Villain>()));
    }

    public class Hero : Interfaces.Hero.GraphQlContract<Domain.Hero>
    {
        private readonly GraphQlJoin<Domain.Hero, Domain.Reputation> reputation =
            GraphQlJoin.JoinSingle<Domain.Hero, Domain.Reputation>(hero => Domain.Data.heroReputation.Where(rep => rep.HeroId == hero.Id).First());
        //GraphQlJoin.Join<Domain.Hero, Domain.Reputation>((originBase) => from t in originBase
        //                                                                 join reputation in Domain.Data.heroReputation on GraphQlJoin.FindOriginal(t).Id equals reputation.HeroId
        //                                                                 select GraphQlJoin.BuildPlaceholder(t, reputation));
        private readonly GraphQlJoin<Domain.Hero, IEnumerable<Domain.Hero>> friends =
            GraphQlJoin.JoinSingle<Domain.Hero, IEnumerable<Domain.Hero>>((original) => from friendId in Domain.Data.friends
                                                                         where original.Id == friendId.Id1
                                                                         join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
                                                                         select friend);
        //GraphQlJoin.Join<Domain.Hero, IEnumerable<Domain.Hero>>((originBase) => from t in originBase
        //                                                                        let friends = (from friendId in Domain.Data.friends
        //                                                                                       where GraphQlJoin.FindOriginal(t).Id == friendId.Id1
        //                                                                                       join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
        //                                                                                       select friend)
        //                                                                        select GraphQlJoin.BuildPlaceholder(t, friends));

        public override IGraphQlResult<string> Faction() =>
            Original.Join(reputation).Resolve((hero, reputation) => reputation.Faction);
        public override IGraphQlResult<IEnumerable<Interfaces.Hero>> Friends() =>
            Original.Join(friends).Resolve((hero, friends) => friends).List(item => item.As<Hero>());
        public override IGraphQlResult<GraphQlId> Id() =>
            Original.Resolve(hero => new GraphQlId(hero.Id));
        public override IGraphQlResult<string> Location(string date) =>
            Original.Resolve(hero => $"Unknown ({date})");
        public override IGraphQlResult<string> Name() =>
            Original.Resolve(hero => hero.Name);
        public override IGraphQlResult<double> Renown() =>
            Original.Join(reputation).Resolve((hero, reputation) => (double)reputation.Renown);
    }

    public class HeroById : Interfaces.Hero.GraphQlContract<string>
    {
        private readonly GraphQlJoin<string, Domain.Hero> hero =
            GraphQlJoin.JoinSingle<string, Domain.Hero>(id => Domain.Data.heroes.Single(hero => id == hero.Id));
        //GraphQlJoin.Join<string, Domain.Hero>((originBase) => from t in originBase
        //                                                      join hero in Domain.Data.heroes on GraphQlJoin.FindOriginal(t) equals hero.Id
        //                                                      select GraphQlJoin.BuildPlaceholder(t, hero));
        private readonly GraphQlJoin<string, Domain.Reputation> reputation =
            GraphQlJoin.JoinSingle<string, Domain.Reputation>(id => Domain.Data.heroReputation.Where(rep => rep.HeroId == id).First());
        //GraphQlJoin.Join<string, Domain.Reputation>((originBase) => from t in originBase
        //                                                                join reputation in Domain.Data.heroReputation on GraphQlJoin.FindOriginal(t) equals reputation.HeroId
        //                                                                select GraphQlJoin.BuildPlaceholder(t, reputation));
        private readonly GraphQlJoin<string, IEnumerable<Domain.Hero>> friends =
            GraphQlJoin.JoinSingle<string, IEnumerable<Domain.Hero>>((id) => from friendId in Domain.Data.friends
                                                              where id == friendId.Id1
                                                              join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
                                                              select friend);
        //GraphQlJoin.Join<string, IEnumerable<Domain.Hero>>((originBase) => from t in originBase
        //                                                                   let friends = (from friendId in Domain.Data.friends
        //                                                                                  where GraphQlJoin.FindOriginal(t) == friendId.Id1
        //                                                                                  join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
        //                                                                                  select friend)
        //                                                                   select GraphQlJoin.BuildPlaceholder(t, friends));

        public override IGraphQlResult<string> Faction() =>
            Original.Join(reputation).Resolve((hero, reputation) => reputation.Faction);
        public override IGraphQlResult<IEnumerable<Interfaces.Hero>> Friends() =>
            Original.Join(friends).Resolve((hero, friends) => friends).List(item => item.As<Hero>());
        public override IGraphQlResult<GraphQlId> Id() =>
            Original.Join(hero).Resolve((_, hero) => new GraphQlId(hero.Id));
        public override IGraphQlResult<string> Location(string date) =>
            Original.Resolve(hero => $"Unknown ({date})");
        public override IGraphQlResult<string> Name() =>
            Original.Join(hero).Resolve((_, hero) => hero.Name);
        public override IGraphQlResult<double> Renown() =>
            Original.Join(reputation).Resolve((hero, reputation) => (double)reputation.Renown);
    }

    public class Villain : Interfaces.Villain.GraphQlContract<Domain.Villain>
    {
        public override IGraphQlResult<string> Goal() =>
            Original.Resolve(villain => villain.Goal);

        public override IGraphQlResult<GraphQlId> Id() =>
            Original.Resolve(villain => new GraphQlId(villain.Id));

        public override IGraphQlResult<string> Name() =>
            Original.Resolve(villain => villain.Name);
    }
}