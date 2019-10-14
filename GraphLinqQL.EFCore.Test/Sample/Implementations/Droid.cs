using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.Sample.Domain;

namespace GraphLinqQL.Sample.Implementations
{
    class Droid : Interfaces.Droid.GraphQlContract<Domain.Droid>
    {
        private readonly StarWarsContext dbContext;

        public Droid(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlScalarResult<IEnumerable<Interfaces.Episode?>> appearsIn(FieldContext fieldContext)
        {
            return Original.Resolve(droid => from appearance in dbContext.Appearances
                                             where appearance.CharacterId == droid.Id
                                             orderby appearance.EpisodeId
                                             select (Interfaces.Episode?)DomainToInterface.ConvertEpisode(appearance.EpisodeId));
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> friends(FieldContext fieldContext)
        {
            return Original.Resolve(droid => from friendship in dbContext.Friendships
                                             where friendship.FromId == droid.Id
                                             select friendship.To).List(UnionMappings.AsCharacterUnion);
        }

        public override IGraphQlObjectResult<Interfaces.FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Id.ToString());

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlScalarResult<string?> primaryFunction(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.PrimaryFunction);
    }
}
