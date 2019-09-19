using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.StarWarsV4.Domain
{
    public class StarWarsContext : DbContext
    {
        public StarWarsContext(DbContextOptions options) : base(options)
        {
        }

#nullable disable
        public DbSet<Film> Films { get; set; }
        public DbSet<FilmCharacter> FilmCharacters { get; set; }
        public DbSet<Person> Characters { get; set; }
#nullable restore

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>(b =>
            {
                b.Property(film => film.Title).IsRequired();
                b.HasKey(film => film.EpisodeId);
                b.HasData(new Film { EpisodeId = 1, Title = "A New Hope" });
            });

            modelBuilder.Entity<Person>(b =>
            {
                b.Property(person => person.Name).IsRequired();
                b.HasKey(person => person.Id);
                b.HasData(new Person { Id = 1, Name = "Luke Skywalker" }, 
                    new Person { Id = 2, Name = "C-3PO" },
                    new Person { Id = 3, Name = "R2-D2" });
            });

            modelBuilder.Entity<FilmCharacter>(b =>
            {
                b.HasKey(fc => new { fc.EpisodeId, fc.PersonId });
                b.HasOne(fc => fc.Film).WithMany(film => film.FilmCharacters).HasForeignKey(fc => fc.EpisodeId);
                b.HasOne(fc => fc.Character).WithMany(person => person.FilmCharacters).HasForeignKey(fc => fc.PersonId);
                b.HasData(new FilmCharacter { EpisodeId = 1, PersonId = 1 },
                    new FilmCharacter { EpisodeId = 1, PersonId = 2 },
                    new FilmCharacter { EpisodeId = 1, PersonId = 3 });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
