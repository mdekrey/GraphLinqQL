using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.Sample.Interfaces;

namespace GraphLinqQL.Sample.Implementations
{
    class FriendsEdge : Interfaces.FriendsEdge.GraphQlContract<Domain.Friendship>
    {
        public override IGraphQlScalarResult<string> cursor(FieldContext fieldContext)
        {
            return Original.Resolve(_ => _.ToId.ToString());
        }

        public override IGraphQlObjectResult<Character?> node(FieldContext fieldContext)
        {
            return Original.Resolve(_ => _.To).AsUnion<Interfaces.Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>());
        }
    }
}
