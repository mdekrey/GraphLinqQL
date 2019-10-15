using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class FriendsEdge : Interfaces.FriendsEdge.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<string> cursor(FieldContext fieldContext) =>
            Original.Resolve(_ => _);

        public override IGraphQlObjectResult<Character?> node(FieldContext fieldContext) =>
            Original.Resolve(id => Domain.Data.humanLookup.ContainsKey(id) ? (object)Domain.Data.humanLookup[id] : Domain.Data.droidLookup[id])
                .AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>());
    }
}
