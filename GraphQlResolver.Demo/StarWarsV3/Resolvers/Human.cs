using System.Collections;
using System.Collections.Generic;
using GraphQlResolver.StarWarsV3.Interfaces;

namespace GraphQlResolver.StarWarsV3.Resolvers
{
    class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        public override IGraphQlResult<IEnumerable<Episode?>> appearsIn()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Character?>?> friends()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.FriendsConnection> friendsConnection(int? first, string? after)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<double?> height(LengthUnit? unit)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> homePlanet()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string> id()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<double?> mass()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string> name()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Starship?>?> starships()
        {
            throw new System.NotImplementedException();
        }
    }

}
