namespace GraphLinqQL.StarWars.Domain
{
    public class Appearance
    {
#nullable disable warnings
        public Episode EpisodeId { get; set; }
        public Film? Film { get; set; }
        public int CharacterId { get; set; }
        public Character? Character { get; set; }
    }
}