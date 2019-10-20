using GraphLinqQL.HandwrittenSamples.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL.HandwrittenSamples.Implementations
{
    public class QueryContract : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> Heroes(int? first) =>
            first == null 
                ? Original.Resolve(root => Domain.DomainData.heroes).List(item => item.AsContract<Hero>())
                : Original.Resolve(root => Domain.DomainData.heroes.Take(first.Value)).List(item => item.AsContract<Hero>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>?> Nulls() =>
            Original.Resolve(root => (IEnumerable<Domain.Hero>?)null).Nullable(nullable => nullable.List(item => item.AsContract<Hero>()));

        public override IGraphQlObjectResult<Interfaces.Hero?> Nohero() =>
            Original.Resolve(root => (Domain.Hero?)null).Nullable(nullable => nullable.AsContract<Hero>());

        public override IGraphQlObjectResult<Interfaces.Hero> Hero() =>
            Original.Resolve(root => Domain.DomainData.heroes[0]).AsContract<Hero>();

        public override IGraphQlObjectResult<Interfaces.Hero> HeroFinalized() =>
            Original.Resolve(root => Domain.DomainData.heroes).List(item => item.AsContract<Hero>()).Only();

        public override IGraphQlObjectResult<Interfaces.Hero> HeroById(string id) =>
            Original.Resolve(root => id).AsContract<HeroById>();

        public override IGraphQlScalarResult<double> Rand() =>
            Original.Resolve(root => 5.0);

        public override IGraphQlObjectResult<IEnumerable<Character>> Characters() =>
            Original.Union(
                _ => _.Resolve(Domain.DomainData.heroes).List(_ => _.AsContract<Hero>() as IGraphQlObjectResult<Character>),
                _ => _.Resolve(Domain.DomainData.villains).List(_ => _.AsContract<Villain>())
            );
    }

    public class Hero : Interfaces.Hero.GraphQlContract<Domain.Hero>
    {
        private readonly GraphQlJoin<Domain.Hero, Domain.Reputation> reputationJoin =
            GraphQlJoin.JoinSingle<Domain.Hero, Domain.Reputation>(hero => Domain.DomainData.heroReputation.Where(rep => rep.HeroId == hero.Id).First());
        //GraphQlJoin.Join<Domain.Hero, Domain.Reputation>((originBase) => from t in originBase
        //                                                                 join reputation in Domain.Data.heroReputation on GraphQlJoin.FindOriginal(t).Id equals reputation.HeroId
        //                                                                 select GraphQlJoin.BuildPlaceholder(t, reputation));
        private readonly GraphQlJoin<Domain.Hero, IEnumerable<Domain.Hero>> friendsJoin =
            GraphQlJoin.JoinSingle<Domain.Hero, IEnumerable<Domain.Hero>>((original) => from friendId in Domain.DomainData.friends
                                                                         where original.Id == friendId.Id1
                                                                         join friend in Domain.DomainData.heroes on friendId.Id2 equals friend.Id
                                                                         select friend);
        //GraphQlJoin.Join<Domain.Hero, IEnumerable<Domain.Hero>>((originBase) => from t in originBase
        //                                                                        let friends = (from friendId in Domain.Data.friends
        //                                                                                       where GraphQlJoin.FindOriginal(t).Id == friendId.Id1
        //                                                                                       join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
        //                                                                                       select friend)
        //                                                                        select GraphQlJoin.BuildPlaceholder(t, friends));

        public override IGraphQlScalarResult<string> Faction() =>
            Original.Join(reputationJoin).Resolve((hero, reputation) => reputation.Faction);
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> Friends() =>
            Original.Join(friendsJoin).Resolve((hero, friends) => friends).List(item => item.AsContract<Hero>());
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> FriendsDeferred() =>
            Original.Join(friendsJoin).Resolve((hero, friends) => friends).Defer(deferred => deferred.List(item => item.AsContract<Hero>()));
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> FriendsTask() =>
            Original.ResolveTask(async hero =>
            {
                await Task.Yield();
                return (from friendId in Domain.DomainData.friends
                        where hero.Id == friendId.Id1
                        join friend in Domain.DomainData.heroes on friendId.Id2 equals friend.Id
                        select friend).ToArray();
            }).List(item => item.AsContract<Hero>());
        public override IGraphQlScalarResult<string> Id() =>
            Original.Resolve(hero => hero.Id);
        public override IGraphQlScalarResult<string> Location(string? date) =>
            Original.Resolve(hero => $"Unknown ({date})");
        public override IGraphQlScalarResult<string> Name() =>
            Original.Resolve(hero => hero.Name);
        public override IGraphQlScalarResult<double> Renown() =>
            Original.Join(reputationJoin).Resolve((hero, reputation) => (double)reputation.Renown);
    }

    public class HeroById : Interfaces.Hero.GraphQlContract<string>
    {
        private readonly GraphQlJoin<string, Domain.Hero> heroJoin =
            GraphQlJoin.JoinSingle<string, Domain.Hero>(id => Domain.DomainData.heroes.Single(hero => id == hero.Id));
        //GraphQlJoin.Join<string, Domain.Hero>((originBase) => from t in originBase
        //                                                      join hero in Domain.Data.heroes on GraphQlJoin.FindOriginal(t) equals hero.Id
        //                                                      select GraphQlJoin.BuildPlaceholder(t, hero));
        private readonly GraphQlJoin<string, Domain.Reputation> reputationJoin =
            GraphQlJoin.JoinSingle<string, Domain.Reputation>(id => Domain.DomainData.heroReputation.Where(rep => rep.HeroId == id).First());
        //GraphQlJoin.Join<string, Domain.Reputation>((originBase) => from t in originBase
        //                                                                join reputation in Domain.Data.heroReputation on GraphQlJoin.FindOriginal(t) equals reputation.HeroId
        //                                                                select GraphQlJoin.BuildPlaceholder(t, reputation));
        private readonly GraphQlJoin<string, IEnumerable<Domain.Hero>> friendsJoin =
            GraphQlJoin.JoinSingle<string, IEnumerable<Domain.Hero>>((id) => from friendId in Domain.DomainData.friends
                                                              where id == friendId.Id1
                                                              join friend in Domain.DomainData.heroes on friendId.Id2 equals friend.Id
                                                              select friend);
        //GraphQlJoin.Join<string, IEnumerable<Domain.Hero>>((originBase) => from t in originBase
        //                                                                   let friends = (from friendId in Domain.Data.friends
        //                                                                                  where GraphQlJoin.FindOriginal(t) == friendId.Id1
        //                                                                                  join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
        //                                                                                  select friend)
        //                                                                   select GraphQlJoin.BuildPlaceholder(t, friends));

        public override IGraphQlScalarResult<string> Faction() =>
            Original.Join(reputationJoin).Resolve((hero, reputation) => reputation.Faction);
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> Friends() =>
            Original.Join(friendsJoin).Resolve((hero, friends) => friends).List(item => item.AsContract<Hero>());
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> FriendsDeferred() =>
            Original.Join(friendsJoin).Resolve((hero, friends) => friends).Defer(deferred => deferred.List(item => item.AsContract<Hero>()));
        public override IGraphQlObjectResult<IEnumerable<Interfaces.Hero>> FriendsTask() =>
            throw new NotImplementedException();
        public override IGraphQlScalarResult<string> Id() =>
            Original.Join(heroJoin).Resolve((_, hero) => hero.Id);
        public override IGraphQlScalarResult<string> Location(string? date) =>
            Original.Resolve(hero => $"Unknown ({date})");
        public override IGraphQlScalarResult<string> Name() =>
            Original.Join(heroJoin).Resolve((_, hero) => hero.Name);
        public override IGraphQlScalarResult<double> Renown() =>
            Original.Join(reputationJoin).Resolve((hero, reputation) => (double)reputation.Renown);
    }

    public class Villain : Interfaces.Villain.GraphQlContract<Domain.Villain>
    {
        public override IGraphQlScalarResult<string> Goal() =>
            Original.Resolve(villain => villain.Goal);

        public override IGraphQlScalarResult<string> Id() =>
            Original.Resolve(villain => villain.Id);

        public override IGraphQlScalarResult<string> Name() =>
            Original.Resolve(villain => villain.Name);
    }
}
