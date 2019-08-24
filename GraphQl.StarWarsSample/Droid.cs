using GraphQlSchema;
using System.Collections.Generic;

#nullable enable

namespace GraphQl.StarWarsSample
{
    public interface Droid : Character
    {
        new IResolver<GraphQlId> Id();
        new IResolver<string> Name();
        new IResolver<IList<Character>> Friends();
        new IResolver<FriendsConnection> FriendsConnection(int? first, GraphQlId? after);
        new IResolver<IList<Episode>> AppearsIn();
        IResolver<string?> PrimaryFunction();
    }
}