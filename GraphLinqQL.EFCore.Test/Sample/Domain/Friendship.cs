namespace GraphLinqQL.Sample.Domain
{
    class Friendship
    {
#nullable disable warnings
        public string FromId { get; set; }
        public Character? From { get; set; }
        public string ToId { get; set; }
        public Character? To { get; set; }
    }
}