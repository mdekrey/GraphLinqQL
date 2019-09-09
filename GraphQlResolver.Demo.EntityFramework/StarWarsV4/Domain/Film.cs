#nullable disable
using System.Collections.Generic;

namespace GraphQlResolver.StarWarsV4.Domain
{
    public class Film
    {
        public int EpisodeId { get; set; }

        public string Title { get; set; }

        public IEnumerable<FilmCharacter> FilmCharacters { get; set; }
    }
}