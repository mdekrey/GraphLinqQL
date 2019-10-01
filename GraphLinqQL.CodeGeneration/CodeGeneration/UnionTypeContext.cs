using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    internal readonly struct UnionTypeContext : ITypeDeclaration
    {
        private readonly UnionTypeDefinition unionTypeDefinition;
        private readonly GraphQLGenerationOptions options;

        public GraphQLGenerationOptions Options => options;

        public string Label => unionTypeDefinition.Name;
        public string TypeName => CSharpNaming.GetTypeName(unionTypeDefinition.Name);

        public string? Description => unionTypeDefinition.Description;

        public string TypeKind => "Union";

        public IEnumerable<string>? ImplementedInterfaces => null;

        public IEnumerable<string>? PossibleTypes => unionTypeDefinition.UnionMembers.Select(u => CSharpNaming.GetTypeName(u.Name));

        public IEnumerable<ObjectFieldContext>? Fields => null;

        public IEnumerable<EnumValueContext>? EnumValues => null;

        public IEnumerable<InputValueContext>? InputFields => null;

        public UnionTypeContext(UnionTypeDefinition unionTypeDefinition, GraphQLGenerationOptions options)
        {
            this.unionTypeDefinition = unionTypeDefinition;
            this.options = options;
        }

        public void Write(TextWriter writer, string indentation)
        {
            writer.WriteLine("// Union - ${Label}");
        }
    }
}