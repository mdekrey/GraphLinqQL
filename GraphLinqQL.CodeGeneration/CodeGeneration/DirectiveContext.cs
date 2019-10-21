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
        private readonly Document document;

        public DirectiveContext(DirectiveDefinition directive, GraphQLGenerationOptions options, Document document)
        {
            this.directive = directive;
            this.options = options;
            this.document = document;
        }

        public string Label => directive.Name;
        public string? Description => directive.Description;
        public IEnumerable<string> Locations =>
            directive.DirectiveLocations.Select(loc => regex.Replace(loc.ToLowerInvariant(), match =>
                match.Value.Replace("_", "").ToUpperInvariant()));

        public IEnumerable<InputValueContext> Arguments
        {
            get
            {
                foreach (var arg in directive.Arguments)
                {
                    yield return new InputValueContext(arg, options, document, directive.GetPropertyName(arg.Name));
                }
            }
        }
    }
}