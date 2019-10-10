using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Sample.Domain
{
    internal class StarWarsContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var lukeSkywalker = 1000;
            var darthVader = 1001;
            var hanSolo = 1002;
            var leiaOrgana = 1003;
            var wilhuffTarkin = 1004;
            var c3p0 = 2000;
            var r2d2 = 2001;

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
                      //AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
                      HomePlanet = "Tatooine",
                      Height = 1.72,
                      Mass = 77,
                      //Starships = new[] { "3001", "3003" },
                  },
                  new Human
                  {
                      Id = darthVader,
                      Name = "Darth Vader",
                      //AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
                      HomePlanet = "Tatooine",
                      Height = 2.02,
                      Mass = 136,
                      //Starships = new[] { "3002" },
                  },
                  new Human
                  {
                      Id = hanSolo,
                      Name = "Han Solo",
                      //Friends = new[] { lukeSkywalker, leiaOrgana, r2d2 },
                      //AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
                      Height = 1.8,
                      Mass = 80,
                      //Starships = new[] { "3000", "3003" },
                  },
                  new Human
                  {
                      Id = leiaOrgana,
                      Name = "Leia Organa",
                      //Friends = new[] { lukeSkywalker, hanSolo, c3p0, r2d2 },
                      //AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
                      HomePlanet = "Alderaan",
                      Height = 1.5,
                      Mass = 49,
                      //Starships = Array.Empty<string>(),
                  },
                  new Human
                  {
                      Id = wilhuffTarkin,
                      Name = "Wilhuff Tarkin",
                      //Friends = new[] { darthVader },
                      //AppearsIn = new[] { Episode.NewHope },
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
                      //Friends = new[] { lukeSkywalker, hanSolo, leiaOrgana, r2d2 },
                      //AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
                      PrimaryFunction = "Protocol",
                  },
                  new Droid
                  {
                      Id = r2d2,
                      Name = "R2-D2",
                      //Friends = new[] { lukeSkywalker, hanSolo, leiaOrgana },
                      //AppearsIn = new[] { Episode.NewHope, Episode.Empire, Episode.Jedi },
                      PrimaryFunction = "Astromech",
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
