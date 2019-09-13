using System.Collections.Generic;

namespace GraphQlResolver.HandwrittenSamples.Domain
{
    public static class Data
    {

        public static readonly IReadOnlyList<Villain> villains = new[]
        {
            new Villain { Id = "THANOS", Name = "Thanos", Goal = "Snap" }
        };

        public static readonly IReadOnlyList<Hero> heroes = new[]
        {
            new Hero { Id = "GUARDIANS-1", Name = "Starlord" },
            new Hero { Id = "ASGUARD-3", Name = "Thor" },
            new Hero { Id = "AVENGERS-1", Name = "Captain America" },
        };
        public static readonly IReadOnlyList<Reputation> heroReputation = new[]
        {
            new Reputation { HeroId = "GUARDIANS-1", Renown = 5, Faction = "Guardians of the Galaxy" },
            new Reputation { HeroId = "ASGUARD-3", Renown = 50, Faction = "Asgardians" },
            new Reputation { HeroId = "AVENGERS-1", Renown = 100, Faction = "Avengers" },
        };
        public static readonly IReadOnlyList<Friend> friends = new Friend[]
        {
            new Friend { Id1 = "AVENGERS-1", Id2 = "ASGUARD-3" },
            new Friend { Id1 = "ASGUARD-3", Id2 = "AVENGERS-1" },
        };
    }
}
