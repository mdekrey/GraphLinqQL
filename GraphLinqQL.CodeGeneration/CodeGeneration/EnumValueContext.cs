using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct EnumValueContext
    {
        private readonly EnumValueDefinition entry;
        private readonly GraphQLGenerationOptions options;

        public EnumValueContext(EnumValueDefinition entry, GraphQLGenerationOptions options)
        {
            this.entry = entry;
            this.options = options;
        }

        public string Name => CSharpNaming.GetPropertyName(entry.EnumValue.TokenValue);
        public string? Description => entry.Description;

        public bool IsDeprecated => entry.Directives.FindObsoleteDirective() != null;
        public string? DeprecationReason => entry.Directives.FindObsoleteDirective()?.ObsoleteReason(options);

    }
}