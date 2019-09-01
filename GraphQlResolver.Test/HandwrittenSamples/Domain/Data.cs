using System.Collections.Generic;

namespace GraphQlResolver.HandwrittenSamples.Domain
{
    public static class Data
    {

        public static readonly IReadOnlyList<Hero> heroes = new[]
        {
            new Hero { Id = "GUARDIANS-1", Name = "Starlord" },
            new Hero { Id = "ASGUARD-3", Name = "Thor" },
        };
        public static readonly IReadOnlyList<Reputation> heroReputation = new[]
        {
            new Reputation { HeroId = "GUARDIANS-1", Renown = 5, Faction = "Guardians of the Galaxy" },
            new Reputation { HeroId = "ASGUARD-3", Renown = 50, Faction = "Asgardians" },
        };
        public static readonly IReadOnlyList<Friend> friends = new Friend[]
        {
        };
    }
}
