using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct EnumValueContext
    {
        private readonly EnumValueDefinition entry;
        private readonly GraphQLGenerationOptions options;
        private readonly Document document;

        public EnumValueContext(EnumValueDefinition entry, GraphQLGenerationOptions options, Document document)
        {
            this.entry = entry;
            this.options = options;
            this.document = document;
        }

        public string Name => CSharpNaming.GetPropertyName(entry.EnumValue.TokenValue);
        public string? Description => entry.Description;

        public bool IsDeprecated => entry.Directives.FindObsoleteDirective() != null;
        public string? DeprecationReason => entry.Directives.FindObsoleteDirective()?.ObsoleteReason(options, document);

    }
}