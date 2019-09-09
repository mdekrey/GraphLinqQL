#nullable disable

namespace GraphQlResolver.StarWarsV4.Domain
{
    public class FilmCharacter
    {
        public int EpisodeId { get; set; }
        public Film Film { get; set; }

        public int PersonId { get; set; }
        public Person Character { get; set; }
    }
}