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
        public override IGraphQlResult<IEnumerable<Interfaces.Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Character?>?> friends(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Id);

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlResult<string?> primaryFunction(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.PrimaryFunction);
    }
}
