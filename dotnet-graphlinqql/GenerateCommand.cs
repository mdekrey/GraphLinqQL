using GraphLinqQL.CodeGeneration;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CA1305 // Specify IFormatProvider

namespace GraphLinqQL
{
    internal class GenerateCommand : CommandLineApplication
    {
        private const string CommandName = "generate";
        private readonly IConsole console;

        public CommandOption InputFileOption { get; }

        public CommandOption OutputFileOption { get; }

        public CommandOption NamespaceOption { get; }

        public CommandOption LanguageVersionOption { get; }

        public GenerateCommand(IConsole console)
        {
            Name = CommandName;
            InputFileOption = Option("-i | --in-file",
                "Input file for the GraphQL schema.",
                CommandOptionType.SingleValue,
                option =>
                {
                    option.IsRequired();
                });

            OutputFileOption = Option("-o | --out-file",
                "Output file where the generated C# will be saved.",
                CommandOptionType.SingleValue,
                option =>
                {
                    option.IsRequired();
                });

            NamespaceOption = Option("-n | --namespace",
                "Namespace containing the generated classes.",
                CommandOptionType.SingleValue);

            LanguageVersionOption = Option("-l | --language-version",
                "Version of C# to be used .",
                CommandOptionType.SingleValue,
                option =>
                {
                    option.Accepts(v => v.Satisfies<System.ComponentModel.DataAnnotations.RangeAttribute>("Must be a positive number", 0.0, double.MaxValue));
                });

            OnExecuteAsync((cancellationToken) => ExecuteAsync(cancellationToken));

            ThrowOnUnexpectedArgument = false;
            this.console = console;
        }

#pragma warning disable CA1801 // Remove unused parameter
        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
#pragma warning restore CA1801 // Remove unused parameter
        {
            // TODO - make CompileFile async and use the cancellationToken
            CompileManager.CompileFile(this.InputFileOption.Value()!, this.OutputFileOption.Value()!, LogError, new GraphQLGenerationOptions { Namespace = NamespaceOption.Value() ?? "Unspecified", LanguageVersion = float.Parse(LanguageVersionOption.Value() ?? "8.0") });

            await Task.Yield();

            return 0;
        }

        private void LogError(CompilerError obj)
        {
            throw new NotImplementedException();
        }
    }
}