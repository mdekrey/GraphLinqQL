using System.IO;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    internal readonly struct UnionTypeContext : ITypeDeclaration
    {
        private readonly UnionTypeDefinition unionTypeDefinition;
        private readonly GraphQLGenerationOptions options;

        public string Label => unionTypeDefinition.Name;
        public string TypeName => CSharpNaming.GetTypeName(unionTypeDefinition.Name);

        public UnionTypeContext(UnionTypeDefinition unionTypeDefinition, GraphQLGenerationOptions options)
        {
            this.unionTypeDefinition = unionTypeDefinition;
            this.options = options;
        }

        public void Write(TextWriter writer, string indentation)
        {
            throw new System.NotImplementedException();
        }
    }
}