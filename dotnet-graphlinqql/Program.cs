using System;
using System.Collections.Generic;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace GraphLinqQL
{
    public static class Program
    {
        public static Task<int> Main(string[] args)
        {
            var servicesProvider = new ServiceCollection()
                .AddSingleton<CommandLineApplication, GenerateCommand>()
                .AddSingleton<IApplicationCommandExecutor, ApplicationCommandsExecutor>()
                .AddSingleton<IConsole, PhysicalConsole>()
                .BuildServiceProvider();


            var executor = servicesProvider.GetRequiredService<IApplicationCommandExecutor>();
            var console = servicesProvider.GetRequiredService<IConsole>();

            try
            {
                return executor.ExecuteAsync(args);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                console.WriteLine(e.Message);
                return Task.FromResult(ReturnCodes.Error);
            }
        }
    }
}
