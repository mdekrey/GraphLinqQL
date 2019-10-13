namespace GraphLinqQL.Sample.Domain
{
#nullable disable warnings
    class Human : Character
    {
        //public string[] Friends { get; set; }
        //public Episode[] AppearsIn { get; set; }
        public string? HomePlanet { get; set; }
        public double Height { get; set; }
        public int? Mass { get; set; }
        //public string[] Starships { get; set; }
    }
}