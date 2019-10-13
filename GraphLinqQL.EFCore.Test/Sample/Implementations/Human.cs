using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.Sample.Domain;
using GraphLinqQL.Sample.Interfaces;

namespace GraphLinqQL.Sample.Implementations
{
    internal class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        private readonly StarWarsContext dbContext;
        private const double MetersToFeet = 3.28084;

        public Human(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlScalarResult<IEnumerable<Interfaces.Episode?>> appearsIn(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> friends(FieldContext fieldContext)
        {
            return Original.Resolve(human => from friendship in dbContext.Friendships
                                             where friendship.FromId == human.Id
                                             select friendship.To).List(UnionMappings.AsCharacterUnion);
        }

        public override IGraphQlObjectResult<FriendsConnection> friendsConnection(FieldContext fieldContext, int? first, string? after)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<double?> height(FieldContext fieldContext, LengthUnit? unit)
        {
            if (unit == LengthUnit.FOOT)
            {
                return Original.Resolve(human => (double?)(human.Height * MetersToFeet));
            }
            else
            {
                return Original.Resolve(human => (double?)human.Height);
            }
        }

        public override IGraphQlScalarResult<string?> homePlanet(FieldContext fieldContext) =>
            Original.Resolve(human => human.HomePlanet);


        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(human => human.Id.ToString());

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(human => human.Name);

        public override IGraphQlScalarResult<double?> mass(FieldContext fieldContext) =>
            Original.Resolve(human => (double?)human.Mass);

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Starship?>?> starships(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }
}