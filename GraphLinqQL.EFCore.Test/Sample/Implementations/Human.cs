using System.Collections.Generic;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.Sample.Interfaces;

namespace GraphLinqQL.Sample.Implementations
{
    internal class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        public override IGraphQlScalarResult<IEnumerable<Interfaces.Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> friends(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<double?> height(FieldContext fieldContext, LengthUnit? unit)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> homePlanet(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);


        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Id.ToString());

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlScalarResult<double?> mass(FieldContext fieldContext) =>
            Original.Resolve(droid => (double?)droid.Mass);

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Starship?>?> starships(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }
}