﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL.StarWars.Domain;

namespace GraphLinqQL.StarWars.Implementations
{
    internal class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        private readonly StarWarsContext dbContext;

        private readonly GraphQlJoin<Domain.Human, IEnumerable<Domain.Appearance>> appearancesJoin;

        public Human(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;

            appearancesJoin =
                GraphQlJoin.JoinSingle<Domain.Human, IEnumerable<Domain.Appearance>>((original) => from appearance in dbContext.Appearances
                                                                                                   where appearance.CharacterId == original.Id
                                                                                                   orderby appearance.EpisodeId
                                                                                                   select appearance);
        }

        public override IGraphQlScalarResult<IEnumerable<Interfaces.Episode?>> appearsIn()
        {
            // using a Join instead of inline Linq to show how reuse could be done
            return Original.Join(appearancesJoin).Resolve((human, appearances) => appearances.Select(appearance => (Interfaces.Episode?)DomainToInterface.ConvertEpisode(appearance.EpisodeId)));
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> friends()
        {
            return Original.Resolve(human => from friendship in dbContext.Friendships
                                             where friendship.FromId == human.Id
                                             select friendship.To).List(UnionMappings.AsCharacterUnion);
        }

        public override IGraphQlObjectResult<Interfaces.FriendsConnection> friendsConnection(int? first, string? after)
        {
            var actualFirst = first ?? 3;
            var actualAfter = after != null ? int.Parse(after) : 0;
            var result = Original.Resolve(human => human.Id).Defer(_ => _.Resolve(humanId => new PaginatedSelection<Domain.Friendship>
            {
                AllData = GetFriendships(humanId),
                SkippedData = GetFriendshipsPaginated(humanId, actualAfter),
                Take = actualFirst
            }).AsContract<FriendsConnection>());
            return result;
        }

        private IQueryable<Domain.Friendship> GetFriendships(int humanId) =>
            from friendship in dbContext.Friendships
            where friendship.FromId == humanId
            orderby friendship.ToId
            select friendship;

        private IQueryable<Domain.Friendship> GetFriendshipsPaginated(int humanId, int after) =>
            (from friendship in dbContext.Friendships
             where friendship.FromId == humanId && friendship.ToId > after
             orderby friendship.ToId
             select friendship);

        public override IGraphQlScalarResult<double?> height(Interfaces.LengthUnit? unit)
        {
            if (unit == Interfaces.LengthUnit.FOOT)
            {
                return Original.Resolve(human => (double?)Conversions.MetersToFeet(human.Height));
            }
            else
            {
                return Original.Resolve(human => (double?)human.Height);
            }
        }

        public override IGraphQlScalarResult<string?> homePlanet() =>
            Original.Resolve(human => human.HomePlanet);


        public override IGraphQlScalarResult<string> id() =>
            Original.Resolve(human => human.Id.ToString());

        public override IGraphQlScalarResult<string> name() =>
            Original.Resolve(human => human.Name);

        public override IGraphQlScalarResult<double?> mass() =>
            Original.Resolve(human => (double?)human.Mass);

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Starship?>?> starships()
        {
            return Original.Resolve(human => from pilot in dbContext.Pilots
                                             where pilot.CharacterId == human.Id
                                             select pilot.Starship).List(_ => _.AsContract<Starship>());
        }
    }
}