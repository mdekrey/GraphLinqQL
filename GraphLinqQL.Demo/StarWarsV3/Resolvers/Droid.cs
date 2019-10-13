using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Droid : Interfaces.Droid.GraphQlContract<Domain.Droid>
    {
        public override IGraphQlScalarResult<IEnumerable<Episode?>> appearsIn(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.AppearsIn.Select(DomainToInterface.ConvertEpisode).Select(ep => (Episode?)ep));

        public override IGraphQlObjectResult<IEnumerable<Character?>?> friends(FieldContext fieldContext) =>
            Original.Resolve(droid => from id in droid.Friends
                                      select Domain.Data.humanLookup.ContainsKey(id) 
                                        ? (object)Domain.Data.humanLookup[id] 
                                        : Domain.Data.droidLookup[id]).List(_ => _.AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()));

        public override IGraphQlObjectResult<Interfaces.FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Id);

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlScalarResult<string?> primaryFunction(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.PrimaryFunction);
    }
}
