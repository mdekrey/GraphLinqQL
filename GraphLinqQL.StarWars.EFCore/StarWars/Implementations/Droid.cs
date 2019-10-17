using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWars.Domain;

namespace GraphLinqQL.StarWars.Implementations
{
    class Droid : Interfaces.Droid.GraphQlContract<Domain.Droid>
    {
        private readonly StarWarsContext dbContext;

        public Droid(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlScalarResult<IEnumerable<Interfaces.Episode?>> appearsIn()
        {
            return Original.Resolve(droid => from appearance in dbContext.Appearances
                                             where appearance.CharacterId == droid.Id
                                             orderby appearance.EpisodeId
                                             select (Interfaces.Episode?)DomainToInterface.ConvertEpisode(appearance.EpisodeId));
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> friends()
        {
            return Original.Resolve(droid => from friendship in dbContext.Friendships
                                             where friendship.FromId == droid.Id
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

        public override IGraphQlScalarResult<string> id() =>
            Original.Resolve(droid => droid.Id.ToString());

        public override IGraphQlScalarResult<string> name() =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlScalarResult<string?> primaryFunction() =>
            Original.Resolve(droid => droid.PrimaryFunction);
    }
}
