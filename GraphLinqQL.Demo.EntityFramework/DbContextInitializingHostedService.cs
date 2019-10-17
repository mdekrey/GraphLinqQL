using System;
using System.Threading;
using System.Threading.Tasks;
using GraphLinqQL.StarWars.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphLinqQL
{
    internal class DbContextInitializingHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public DbContextInitializingHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StarWarsContext>();
            await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}