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
        public string Label => enumTypeDefinition.Name;
        public string TypeName => CSharpNaming.GetTypeName(enumTypeDefinition.Name);

        public void Write(TextWriter writer, string indentation)
        {
            writer.WriteLine($"// TODO - enum {TypeName}");
        }
    }
}