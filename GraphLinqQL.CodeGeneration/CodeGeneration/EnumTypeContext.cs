using System.Collections.Generic;
using System.IO;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct EnumTypeContext : ITypeDeclaration
    {
        private readonly EnumTypeDefinition enumTypeDefinition;
        private readonly GraphQLGenerationOptions options;

        public EnumTypeContext(EnumTypeDefinition enumTypeDefinition, GraphQLGenerationOptions options)
        {
            this.enumTypeDefinition = enumTypeDefinition;
            this.options = options;
        }
        public GraphQLGenerationOptions Options => options;

        public string Label => enumTypeDefinition.Name;
        public string TypeName => CSharpNaming.GetTypeName(enumTypeDefinition.Name);

        public string? Description => enumTypeDefinition.Description;

        public string TypeKind => "Enum";

        public IEnumerable<string>? ImplementedInterfaces => null;

        public IEnumerable<string>? PossibleTypes => null;

        public IEnumerable<ObjectFieldContext>? Fields => null;

        public IEnumerable<EnumValueContext>? EnumValues
        {
            get
            {
                foreach (var entry in enumTypeDefinition.EnumValues)
                {
                    yield return new EnumValueContext(entry, options);
                }
            }
        }

        public IEnumerable<InputValueContext>? InputFields => null;

        public void Write(TextWriter writer, string indentation)
        {
            Templates.EnumTypeGenerator.RenderEnumType(this, writer, indentation);
        }
    }
}