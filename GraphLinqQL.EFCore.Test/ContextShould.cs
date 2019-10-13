using GraphLinqQL.Sample.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GraphLinqQL.EFCore.Test
{
    public class ContextShould
    {
        [Fact]
        public async Task HaveADefaultSetup()
        {
            using var context = CreateStarWarsContext();
            var droids = await context.Characters.ToListAsync().ConfigureAwait(false);
            Assert.Contains(droids, droid => droid.Name == "C-3PO");
            Assert.Contains(droids, droid => droid.Name == "R2-D2");

            var friendships = await context.Friendships.Include(f => f.From).Include(f => f.To).ToListAsync().ConfigureAwait(false);
            Assert.Contains(friendships, friendship => friendship.From!.Name == "Luke Skywalker" && friendship.To!.Name == "Han Solo");
            Assert.Contains(friendships, friendship => friendship.From!.Name == "Han Solo" && friendship.To!.Name == "Luke Skywalker");

            var films = await context.Films.Include(f => f.Hero).ToListAsync().ConfigureAwait(false);
            Assert.Contains(films, film => film.EpisodeId == Episode.NewHope && film.Hero!.Name == "R2-D2");
            Assert.Contains(films, film => film.EpisodeId == Episode.Empire && film.Hero!.Name == "Luke Skywalker");

        }

        private StarWarsContext CreateStarWarsContext()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var inMemorySqlite = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2000 // Dispose objects before losing scope
            inMemorySqlite.Open();
            var options = new DbContextOptionsBuilder<StarWarsContext>()
                   .UseSqlite(inMemorySqlite)
                   .Options;

            var result = new StarWarsContext(options);
            result.Database.EnsureCreated();
            return result;
        }
    }
}
