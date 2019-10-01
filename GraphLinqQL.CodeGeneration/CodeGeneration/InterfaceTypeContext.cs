using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct InterfaceTypeContext : ITypeDeclaration
    {
        private readonly InterfaceTypeDefinition declaration;
        private readonly GraphQLGenerationOptions options;
        private readonly Document document;

        public InterfaceTypeContext(InterfaceTypeDefinition interfaceTypeDefinition, GraphQLGenerationOptions options, Document document)
        {
            this.declaration = interfaceTypeDefinition;
            this.options = options;
            this.document = document;
        }

        public GraphQLGenerationOptions Options => options;

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
                    yield return new ObjectFieldContext(field, options, document);
                }
            }
        }

        public string TypeKind => "Interface";

        public IEnumerable<string>? ImplementedInterfaces => null;

        public IEnumerable<string>? PossibleTypes => null;

        public IEnumerable<EnumValueContext>? EnumValues => null;

        public IEnumerable<InputObjectFieldContext>? InputFields => null;
    }
}