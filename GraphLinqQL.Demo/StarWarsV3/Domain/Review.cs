namespace GraphLinqQL.StarWarsV3.Domain
{
#nullable disable
    internal class Review
    {
        public Episode Episode { get; set; }
        public int Stars { get; set; }
        public string Commentary { get; set; }
    }
}