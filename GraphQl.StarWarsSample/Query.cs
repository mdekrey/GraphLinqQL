using GraphQlSchema;
using System.Collections.Generic;

#nullable enable

namespace GraphQl.StarWarsSample
{
    public interface Query
    {
        IResolver<Character> Hero(Episode? episode);
        IResolver<IList<Review>> Reviews(Episode episode);
        IResolver<IList<SearchResult>> Search(string? text = null);
        IResolver<Character> Character(GraphQlId id);
        IResolver<Droid> Droid(GraphQlId id);
        IResolver<Human> Human(GraphQlId id);
        IResolver<Starship> Starship(GraphQlId id);
    }
}