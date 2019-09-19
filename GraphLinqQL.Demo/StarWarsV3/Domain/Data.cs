using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.StarWarsV3.Domain
{
    class Data
    {
        public static readonly IReadOnlyList<Human> humans = new[] {
          new Human {
            Id = "1000",
            Name = "Luke Skywalker",
            Friends = new[] { "1002", "1003", "2000", "2001" },
            AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
            HomePlanet = "Tatooine",
            Height = 1.72,
            Mass = 77,
            Starships = new[] { "3001", "3003" },
          },
          new Human {
            Id = "1001",
            Name = "Darth Vader",
            Friends = new[] { "1004" },
            AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
            HomePlanet = "Tatooine",
            Height = 2.02,
            Mass = 136,
            Starships = new[] { "3002" },
          },
          new Human {
            Id = "1002",
            Name = "Han Solo",
            Friends = new[] { "1000", "1003", "2001" },
            AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
            Height = 1.8,
            Mass = 80,
            Starships = new[] { "3000", "3003" },
          },
          new Human {
            Id = "1003",
            Name = "Leia Organa",
            Friends = new[] { "1000", "1002", "2000", "2001" },
            AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
            HomePlanet = "Alderaan",
            Height = 1.5,
            Mass = 49,
            Starships = Array.Empty<string>(),
          },
          new Human {
            Id = "1004",
            Name = "Wilhuff Tarkin",
            Friends = new[] { "1001" },
            AppearsIn = new[] { Episode.NewHope },
            Height = 1.8,
            Mass = null,
            Starships = Array.Empty<string>(),
          },
        };

        public static readonly IDictionary<string, Human> humanLookup = humans.ToDictionary(human => human.Id);

        public static readonly IReadOnlyList<Droid> droids = new[] {
          new Droid {
            Id = "2000",
            Name = "C-3PO",
            Friends = new[] { "1000", "1002", "1003", "2001" },
            AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
            PrimaryFunction = "Protocol",
          },
          new Droid {
            Id = "2001",
            Name = "R2-D2",
            Friends = new[] { "1000", "1002", "1003" },
            AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
            PrimaryFunction = "Astromech",
          },
        };
        public static readonly IDictionary<string, Droid> droidLookup = droids.ToDictionary(droid => droid.Id);

        public static readonly IReadOnlyList<Starship> starships = new[] {
          new Starship {
            Id = "3000",
            Name = "Millenium Falcon",
            Length = 34.37,
          },
          new Starship {
            Id = "3001",
            Name = "X-Wing",
            Length = 12.5,
          },
          new Starship {
            Id = "3002",
            Name = "TIE Advanced x1",
            Length = 9.2,
          },
          new Starship {
            Id = "3003",
            Name = "Imperial shuttle",
            Length = 20,
          },
        };
        public static readonly IDictionary<string, Starship> starshipLookup = starships.ToDictionary(starship => starship.Id);

        public readonly static IDictionary<Episode, List<Review>> reviews = new Dictionary<Episode, List<Review>> {
          { Episode.NewHope, new List<Review>() },
          { Episode.Empire, new List<Review>() },
          { Episode.Jedi, new List<Review>() }
        };
    }
}
