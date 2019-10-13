using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.Sample.Interfaces;

namespace GraphLinqQL.Sample.Implementations
{
    class Droid : Interfaces.Droid.GraphQlContract<Domain.Droid>
    {
        public override IGraphQlScalarResult<IEnumerable<Interfaces.Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> friends(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
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
