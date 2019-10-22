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
        public override IGraphQlScalarResult<IEnumerable<Episode?>> AppearsIn() =>
            this.Resolve(droid => droid.AppearsIn.Select(DomainToInterface.ConvertEpisode).Select(ep => (Episode?)ep));

        public override IGraphQlObjectResult<IEnumerable<Character?>?> Friends() =>
            this.Resolve(droid => from id in droid.Friends
                                      select Domain.Data.humanLookup.ContainsKey(id) 
                                        ? (object)Domain.Data.humanLookup[id] 
                                        : Domain.Data.droidLookup[id]).List(_ => _.AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()));

        public override IGraphQlObjectResult<Interfaces.FriendsConnection> FriendsConnection(int? first, string? after) =>
            this.Resolve(droid => new FriendsConnection.Data(droid.Friends, first, after)).AsContract<FriendsConnection>();

        public override IGraphQlScalarResult<string> Id() =>
            this.Resolve(droid => droid.Id);

        public override IGraphQlScalarResult<string> Name() =>
            this.Resolve(droid => droid.Name);

        public override IGraphQlScalarResult<string?> PrimaryFunction() =>
            this.Resolve(droid => droid.PrimaryFunction);
    }
}
