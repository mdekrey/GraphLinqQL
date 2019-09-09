using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQlResolver.StarWarsV4.Domain
{
    public class StarWarsContextFactory : IDesignTimeDbContextFactory<StarWarsContext>
    {
        public StarWarsContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("configuration.local.json", optional: false, reloadOnChange: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<StarWarsContext>();
            optionsBuilder.UseSqlServer(config["DefaultConnection"]);

            return new StarWarsContext(optionsBuilder.Options);
        }
    }
}
