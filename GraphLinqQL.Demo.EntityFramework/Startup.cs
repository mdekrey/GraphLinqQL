using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GraphLinqQL.Execution;
using GraphLinqQL.StarWars.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace GraphLinqQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope - this gets added to the DbContext and used for the duration of the app
#pragma warning disable IDE0067
            var inMemorySqlite = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2000 // Dispose objects before losing scope
            inMemorySqlite.Open();

            services.AddDbContext<StarWarsContext>(options => {
                options.UseSqlite(inMemorySqlite);
            });

            services.AddGraphQl<StarWars.Interfaces.TypeResolver>(typeof(StarWars.Implementations.Query), options =>
            {
                options.Mutation = typeof(StarWars.Implementations.Mutation);
                options.AddIntrospection();
            });

            services.AddHostedService<DbContextInitializingHostedService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Try out the /star-wars-v3/graphql endpoint!").ConfigureAwait(false);
                });

                endpoints.UseGraphQl("/star-wars-v3/graphql");
            });
        }
    }
}
