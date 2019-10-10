using System.Collections.Generic;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.Sample.Interfaces;

namespace GraphLinqQL.Sample.Implementations
{
    internal class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        public override IGraphQlResult<IEnumerable<Interfaces.Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Character?>?> friends(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<double?> height(FieldContext fieldContext, LengthUnit? unit)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> homePlanet(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);


        public override IGraphQlResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Id.ToString());

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlResult<double?> mass(FieldContext fieldContext) =>
            Original.Resolve(droid => (double?)droid.Mass);

        public override IGraphQlResult<IEnumerable<Interfaces.Starship?>?> starships(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }
}