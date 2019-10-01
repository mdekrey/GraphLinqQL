using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct InterfaceTypeContext : ITypeDeclaration
    {
        private readonly InterfaceTypeDefinition declaration;
        private readonly GraphQLGenerationOptions options;

        public InterfaceTypeContext(InterfaceTypeDefinition interfaceTypeDefinition, GraphQLGenerationOptions options)
        {
            this.declaration = interfaceTypeDefinition;
            this.options = options;
        }

        public void Write(TextWriter writer, string indentation)
        {
            Templates.InterfaceTypeGenerator.RenderInterfaceType(this, writer, indentation);
        }

        public string Label => declaration.Name;
        public string TypeName => CSharpNaming.GetTypeName(declaration.Name);

        public string? Description => declaration.Description;

        public IEnumerable<ObjectFieldContext> Fields
        {
            get
            {
                foreach (var field in declaration.Fields)
                {
                    yield return new ObjectFieldContext(field, options);
                }
            }
        }
    }
}