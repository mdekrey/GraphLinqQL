using GraphQlSchema;
using System.Collections.Generic;

namespace GraphQl.StarWarsSample
{
    public interface Human : Character
    {
        new IResolver<GraphQlId> Id();
        new IResolver<string> Name();
        IResolver<string> HomePlanet();
        IResolver<double?> Height(LengthUnit? unit);
        IResolver<double?> Mass();

        new IResolver<IList<Character>> Friends();
        new IResolver<FriendsConnection> FriendsConnection(int? first, GraphQlId? after);
        new IResolver<IList<Episode>> AppearsIn();
        IResolver<IList<Starship>> Starships();
    }
}