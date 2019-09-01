using GraphQlSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQlResolver.HandwrittenSamples.Implementations
{

    public class Query : Interfaces.Query, IGraphQlAccepts<GraphQlRoot>
    {
#nullable disable
        public IGraphQlResultFactory<GraphQlRoot> Original { get; set; }
#nullable restore

        public IGraphQlResult<IEnumerable<Interfaces.Hero>> Heroes() =>
            Original.Resolve(root => Domain.Data.heroes).ConvertableList().As<Hero>();

        public IGraphQlResult<double> Rand() =>
            Original.Resolve(root => 5.0);
    }

    public class Hero : Interfaces.Hero, IGraphQlAccepts<Domain.Hero>
    {
#nullable disable
        public IGraphQlResultFactory<Domain.Hero> Original { get; set; }
#nullable restore

        private readonly GraphQlJoin<Domain.Hero, Domain.Reputation> reputation =
            GraphQlJoin.Join<Domain.Hero, Domain.Reputation>((originBase) => from t in originBase
                                                                             join reputation in Domain.Data.heroReputation on GraphQlJoin.FindOriginal(t).Id equals reputation.HeroId
                                                                             select GraphQlJoin.BuildPlaceholder(t, reputation));
        private readonly GraphQlJoin<Domain.Hero, IEnumerable<Domain.Hero>> friends =
            GraphQlJoin.Join<Domain.Hero, IEnumerable<Domain.Hero>>((originBase) => from t in originBase
                                                                                    let friends = (from friendId in Domain.Data.friends
                                                                                                   where GraphQlJoin.FindOriginal(t).Id == friendId.Id1
                                                                                                   join friend in Domain.Data.heroes on friendId.Id2 equals friend.Id
                                                                                                   select friend)
                                                                                    select GraphQlJoin.BuildPlaceholder(t, friends));

        public IGraphQlResult<string> Faction() =>
            Original.Join(reputation).Resolve((hero, reputation) => reputation.Faction);
        public IGraphQlResult<IEnumerable<Interfaces.Hero>> Friends() =>
            Original.Join(friends).Resolve((hero, friends) => friends).ConvertableList().As<Hero>();
        public IGraphQlResult<GraphQlId> Id() =>
            Original.Resolve(hero => new GraphQlId(hero.Id));
        public IGraphQlResult<string> Location(string date) =>
            Original.Resolve(hero => $"Unknown ({date})");
        public IGraphQlResult<string> Name() =>
            Original.Resolve(hero => hero.Name);
        public IGraphQlResult<double> Renown() =>
            Original.Join(reputation).Resolve((hero, reputation) => (double)reputation.Renown);
    }
}
