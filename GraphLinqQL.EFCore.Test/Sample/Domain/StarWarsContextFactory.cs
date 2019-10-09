using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.Sample.Domain
{
    internal class StarWarsContextFactory : IDesignTimeDbContextFactory<StarWarsContext>
    {
        public StarWarsContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<StarWarsContext>();
            optionsBuilder
                   .UseSqlite("Data Source=:memory:");

            return new StarWarsContext(optionsBuilder.Options);
        }
    }
}
