using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        public override IGraphQlResult<IEnumerable<Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Character?>?> friends(FieldContext fieldContext) =>
            Original.Resolve(human => human.Friends.Where(id => Domain.Data.humanLookup.ContainsKey(id)).Select(id => Domain.Data.humanLookup[id])).List(_ => _.AsContract<Human>())
                .Union<IEnumerable<Character?>?>(Original.Resolve(human => human.Friends.Where(id => Domain.Data.droidLookup.ContainsKey(id)).Select(id => Domain.Data.droidLookup[id])).List(_ => _.AsContract<Droid>()));

        public override IGraphQlResult<Interfaces.FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<double?> height(FieldContext fieldContext, LengthUnit? unit)
        {
            var unitFactor = (unit ?? LengthUnit.METER) == LengthUnit.METER ? 1 : 3.28084;
            return Original.Resolve<double?>(human => human.Height * unitFactor);
        }

        public override IGraphQlResult<string?> homePlanet(FieldContext fieldContext) =>
            Original.Resolve(human => human.HomePlanet);

        public override IGraphQlResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(human => human.Id);

        public override IGraphQlResult<double?> mass(FieldContext fieldContext) =>
            Original.Resolve<double?>(human => human.Mass);

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(human => human.Name);

        public override IGraphQlResult<IEnumerable<Interfaces.Starship?>?> starships(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }

}
