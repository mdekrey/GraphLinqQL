#nullable disable
using System.Collections.Generic;
using System.Linq;

namespace GraphQlResolver.StarWarsV4.Domain
{
    public class Film
    {
        public int EpisodeId { get; set; }

        public string Title { get; set; }

        public IQueryable<FilmCharacter> FilmCharacters { get; set; }
    }
}