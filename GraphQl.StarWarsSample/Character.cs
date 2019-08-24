using GraphQlSchema;
using System.Collections.Generic;

namespace GraphQl.StarWarsSample
{
    public interface Character
    {
        IResolver<GraphQlId> Id();
        IResolver<string> Name();
        IResolver<IList<Character>> Friends();
        IResolver<FriendsConnection> FriendsConnection(int? first, GraphQlId? after);
        IResolver<IList<Episode>> AppearsIn();
    }
}