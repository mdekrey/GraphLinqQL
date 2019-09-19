namespace GraphLinqQL.StarWarsV3.Domain
{
#nullable disable
    internal class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Friends { get; set; }
        public Episode[] AppearsIn { get; set; }
        public string PrimaryFunction { get; set; }
    }
}