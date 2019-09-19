namespace GraphLinqQL.StarWarsV3.Domain
{
#nullable disable
    internal class Human
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Friends { get; set; }
        public Episode[] AppearsIn { get; set; }
        public string HomePlanet { get; set; }
        public double Height { get; set; }
        public int? Mass { get; set; }
        public string[] Starships { get; set; }
    }
}