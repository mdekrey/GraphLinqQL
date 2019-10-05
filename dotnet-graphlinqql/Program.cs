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
#if NET45
        public static int Main(string[] args)
#else
        public static Task<int> Main(string[] args)
#endif
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
#if NET45
                return executor.ExecuteAsync(args).Result;
#else
                return executor.ExecuteAsync(args);
#endif
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                console.WriteLine(e.Message);
#if NET45
                return ReturnCodes.Error;
#else
                return Task.FromResult(ReturnCodes.Error);
#endif
            }
        }
    }
}
