using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        public override IGraphQlScalarResult<IEnumerable<Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<IEnumerable<Character?>?> friends(FieldContext fieldContext) =>
            Original.Resolve(human => from id in human.Friends
                                      select Domain.Data.humanLookup.ContainsKey(id)
                                        ? (object)Domain.Data.humanLookup[id]
                                        : Domain.Data.droidLookup[id]).List(_ => _.AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()));

        public override IGraphQlObjectResult<Interfaces.FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<double?> height(FieldContext fieldContext, LengthUnit? unit)
        {
            var unitFactor = (unit ?? LengthUnit.METER) == LengthUnit.METER ? 1 : 3.28084;
            return Original.Resolve<double?>(human => human.Height * unitFactor);
        }

        public override IGraphQlScalarResult<string?> homePlanet(FieldContext fieldContext) =>
            Original.Resolve(human => human.HomePlanet);

        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(human => human.Id);

        public override IGraphQlScalarResult<double?> mass(FieldContext fieldContext) =>
            Original.Resolve<double?>(human => human.Mass);

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(human => human.Name);

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Starship?>?> starships(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }

}
