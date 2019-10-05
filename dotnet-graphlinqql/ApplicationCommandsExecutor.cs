using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public sealed class ApplicationCommandsExecutor : CommandLineApplication, IApplicationCommandExecutor
    {
        private readonly IEnumerable<CommandLineApplication> _subcommands;

        public ApplicationCommandsExecutor(IEnumerable<CommandLineApplication> commands)
        {
            _subcommands = commands ?? throw new ArgumentNullException(nameof(commands));

            ConfigureCommandLineApplication();
        }

        private void ConfigureCommandLineApplication()
        {
            Name = "graphlinqql";

            Commands.AddRange(_subcommands);

            Conventions.UseDefaultConventions();

            ThrowOnUnexpectedArgument = false;
        }

        Task<int> IApplicationCommandExecutor.ExecuteAsync(string[] args)
        {
            var selectedCommand = Parse(args).SelectedCommand;

            if (selectedCommand.IsShowingInformation)
            {
                return Task.FromResult(ReturnCodes.Success);
            }

            if (selectedCommand.Name == Name)
            {
                ShowHint();
                return Task.FromResult(ReturnCodes.Error);
            }

            return selectedCommand.ExecuteAsync(args);
        }
    }
}
