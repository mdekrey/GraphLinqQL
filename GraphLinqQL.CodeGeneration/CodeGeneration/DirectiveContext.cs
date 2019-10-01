using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
#pragma warning disable CA1308 // Normalize strings to uppercase

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct DirectiveContext
    {
        private static Regex regex = new Regex("(^|_).");

        private readonly DirectiveDefinition directive;
        private readonly GraphQLGenerationOptions options;

        public DirectiveContext(DirectiveDefinition directive, GraphQLGenerationOptions options)
        {
            this.directive = directive;
            this.options = options;
        }

        public string Label => directive.Name;
        public string? Description => directive.Description;
        public IEnumerable<string> Locations =>
            directive.DirectiveLocations.Select(loc => regex.Replace(loc.ToLowerInvariant(), match =>
#if !NET45
                match.Value.Replace("_", "", System.StringComparison.InvariantCulture)
#else
                match.Value.Replace("_", "")
#endif
                    .ToUpperInvariant()));

        public IEnumerable<InputValueContext> Arguments
        {
            get
            {
                foreach (var arg in directive.Arguments)
                {
                    yield return new InputValueContext(arg, options);
                }
            }
        }
    }
}