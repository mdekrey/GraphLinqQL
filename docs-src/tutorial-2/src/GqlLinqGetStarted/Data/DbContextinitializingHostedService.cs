using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GqlLinqGetStarted.Data
{
    // ###HostedService
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
            var dbContext = scope.ServiceProvider.GetRequiredService<Data.BloggingContext>();
            await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
    // HostedService###
}
