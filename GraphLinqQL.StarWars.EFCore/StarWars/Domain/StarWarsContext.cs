using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.StarWars.Domain
{
    public class StarWarsContext : DbContext
    {
#nullable disable warnings
        public StarWarsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Human> Humans { get; set; }
        public DbSet<Droid> Droids { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Appearance> Appearances { get; set; }
        public DbSet<Starship> Starships { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var lukeSkywalker = 1000;
            var darthVader = 1001;
            var hanSolo = 1002;
            var leiaOrgana = 1003;
            var wilhuffTarkin = 1004;
            var c3p0 = 2000;
            var r2d2 = 2001;
            var milleniumFalcon = 3000;
            var xWing = 3001;
            var tieAdvanced = 3002;
            var imperialShuttle = 3003;

            modelBuilder.Entity<Character>(b =>
            {
                b.HasKey(character => character.Id);
            });
            modelBuilder.Entity<Human>(b =>
            {
                b.HasData(
                  new Human
                  {
                      Id = lukeSkywalker,
                      Name = "Luke Skywalker",
                      HomePlanet = "Tatooine",
                      Height = 1.72,
                      Mass = 77,
                      //Starships = new[] { xWing, imperialShuttle },
                  },
                  new Human
                  {
                      Id = darthVader,
                      Name = "Darth Vader",
                      HomePlanet = "Tatooine",
                      Height = 2.02,
                      Mass = 136,
                      //Starships = new[] { tieAdvanced },
                  },
                  new Human
                  {
                      Id = hanSolo,
                      Name = "Han Solo",
                      Height = 1.8,
                      Mass = 80,
                      //Starships = new[] { milleniumFalcon, imperialShuttle },
                  },
                  new Human
                  {
                      Id = leiaOrgana,
                      Name = "Leia Organa",
                      HomePlanet = "Alderaan",
                      Height = 1.5,
                      Mass = 49,
                      //Starships = Array.Empty<string>(),
                  },
                  new Human
                  {
                      Id = wilhuffTarkin,
                      Name = "Wilhuff Tarkin",
                      Height = 1.8,
                      Mass = null,
                      //Starships = Array.Empty<string>(),
                  });
            });

            modelBuilder.Entity<Droid>(b =>
            {
                b.HasData(
                  new Droid
                  {
                      Id = c3p0,
                      Name = "C-3PO",
                      PrimaryFunction = "Protocol",
                  },
                  new Droid
                  {
                      Id = r2d2,
                      Name = "R2-D2",
                      PrimaryFunction = "Astromech",
                  }
                );
            });

            modelBuilder.Entity<Starship>(b =>
            {
                b.HasKey(s => s.Id);
                b.HasData(
                  new Starship
                  {
                      Id = milleniumFalcon,
                      Name = "Millenium Falcon",
                      Length = 34.37,
                  },
                  new Starship
                  {
                      Id = xWing,
                      Name = "X-Wing",
                      Length = 12.5,
                  },
                  new Starship
                  {
                      Id = tieAdvanced,
                      Name = "TIE Advanced x1",
                      Length = 9.2,
                  },
                  new Starship
                  {
                      Id = imperialShuttle,
                      Name = "Imperial Shuttle",
                      Length = 20,
                  }
                );
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasKey(f => new { f.FromId, f.ToId });
                b.HasOne<Character>(f => f.From).WithMany(c => c.Friendships).HasForeignKey(f => f.FromId).HasPrincipalKey(c => c.Id).OnDelete(DeleteBehavior.Restrict);
                b.HasOne<Character>(f => f.To).WithMany(/* friendships */).HasForeignKey(f => f.ToId).HasPrincipalKey(c => c.Id).OnDelete(DeleteBehavior.Restrict);
                b.HasData((from character in new[]
                            {
                                new { @from = lukeSkywalker, to = new[] { hanSolo, leiaOrgana, c3p0, r2d2 } },
                                new { @from = darthVader, to = new[] { wilhuffTarkin } },
                                new { @from = hanSolo, to = new[] { lukeSkywalker, leiaOrgana, r2d2 } },
                                new { @from = leiaOrgana, to = new[] { lukeSkywalker, hanSolo, c3p0, r2d2 } },
                                new { @from = wilhuffTarkin, to = new[] { darthVader } },
                                new { @from = c3p0, to = new[] { lukeSkywalker, hanSolo, leiaOrgana, r2d2 } },
                                new { @from = r2d2, to = new[] { lukeSkywalker, hanSolo, leiaOrgana } },
                            }
                           from friend in character.to
                           select new Friendship { FromId = character.@from, ToId = friend }).ToArray());
            });

            modelBuilder.Entity<Film>(b =>
            {
                b.HasKey(f => f.EpisodeId);
                b.HasOne(f => f.Hero).WithMany().HasForeignKey(f => f.HeroId);
                b.HasData(new Film { EpisodeId = Episode.NewHope, Title = "A New Hope", HeroId = r2d2 },
                    new Film { EpisodeId = Episode.Empire, Title = "The Empire Strikes Back", HeroId = lukeSkywalker },
                    new Film { EpisodeId = Episode.Jedi, Title = "The Return of the Jedi", HeroId = r2d2 });
            });

            modelBuilder.Entity<Appearance>(b =>
            {
                b.HasKey(a => new { a.EpisodeId, a.CharacterId });
                b.HasOne(a => a.Film).WithMany().HasForeignKey(a => a.EpisodeId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(a => a.Character).WithMany().HasForeignKey(a => a.CharacterId).OnDelete(DeleteBehavior.Restrict);
                b.HasData((from character in new[]
                            {
                                new { id = lukeSkywalker, films = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi } },
                                new { id = darthVader, films = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi } },
                                new { id = hanSolo, films = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi } },
                                new { id = leiaOrgana, films = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi } },
                                new { id = wilhuffTarkin, films = new[] { Episode.NewHope } },
                                new { id = c3p0, films = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi } },
                                new { id = r2d2, films = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi } },
                            }
                           from appearance in character.films
                           select new Appearance { CharacterId = character.id, EpisodeId = appearance }).ToArray());
            });

            modelBuilder.Entity<Pilot>(b =>
            {
                b.HasKey(p => new { p.CharacterId, p.StarshipId });
                b.HasOne(p => p.Character).WithMany().HasForeignKey(a => a.CharacterId).OnDelete(DeleteBehavior.Restrict);
                b.HasData((from character in new[]
                            {
                                new { id = lukeSkywalker, starships = new[] { xWing, imperialShuttle } },
                                new { id = darthVader, starships = new[] { tieAdvanced } },
                                new { id = hanSolo, starships = new[] { milleniumFalcon, imperialShuttle } },
                            }
                           from starship in character.starships
                           select new Pilot { CharacterId = character.id, StarshipId = starship }).ToArray());
            });

            modelBuilder.Entity<Review>(b =>
            {
                b.HasKey(r => r.ReviewId);
                b.Property(r => r.ReviewId).ValueGeneratedOnAdd();
                b.HasIndex(r => new { r.Episode, r.Stars });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
