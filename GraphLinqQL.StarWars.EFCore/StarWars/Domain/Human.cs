namespace GraphLinqQL.StarWars.Domain
{
#nullable disable warnings
    public class Human : Character
    {
        public string? HomePlanet { get; set; }
        public double Height { get; set; }
        public int? Mass { get; set; }
    }
}