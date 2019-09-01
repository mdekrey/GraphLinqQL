using System;
using System.Text;

namespace GraphQlResolver.HandwrittenSamples.Domain
{
    public class Hero
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Reputation
    {
        public string HeroId { get; set; }
        public float Renown { get; set; }
        public string Faction { get; set; }
    }

    public class Friend
    {
        public string Id1 { get; set; }
        public string Id2 { get; set; }
    }
}
