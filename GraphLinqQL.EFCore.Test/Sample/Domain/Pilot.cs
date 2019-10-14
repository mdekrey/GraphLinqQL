namespace GraphLinqQL.Sample.Domain
{
    class Pilot
    {
#nullable disable warnings
        public int CharacterId { get; set; }
        public Human? Character { get; set; }
        public int StarshipId { get; set; }
        public Starship? Starship { get; set; }
    }
}