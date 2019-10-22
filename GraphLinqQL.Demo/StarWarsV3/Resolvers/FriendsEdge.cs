using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class FriendsEdge : Interfaces.FriendsEdge.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<string> Cursor() =>
            this.Resolve(_ => _);

        public override IGraphQlObjectResult<Character?> Node() =>
            this.Resolve(id => Domain.Data.humanLookup.ContainsKey(id) ? (object)Domain.Data.humanLookup[id] : Domain.Data.droidLookup[id])
                .AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>());
    }
}
