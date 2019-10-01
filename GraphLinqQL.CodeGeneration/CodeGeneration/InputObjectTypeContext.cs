using System;
using System.Collections.Generic;
using System.IO;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct InputObjectTypeContext : ITypeDeclaration
    {
        private readonly InputObjectTypeDefinition declaration;
        private readonly GraphQLGenerationOptions options;

        public InputObjectTypeContext(InputObjectTypeDefinition declaration, GraphQLGenerationOptions options)
        {
            this.declaration = declaration;
            this.options = options;
        }

        public GraphQLGenerationOptions Options => options;

        public string Label => declaration.Name;
        public string TypeName => CSharpNaming.GetTypeName(declaration.Name);

        public string? Description => declaration.Description;

        public string TypeKind => "InputObject";

        public IEnumerable<string>? ImplementedInterfaces => null;

        public IEnumerable<string>? PossibleTypes => null;

        IEnumerable<ObjectFieldContext>? ITypeDeclaration.Fields => null;

        public IEnumerable<EnumValueContext>? EnumValues => null;

        public IEnumerable<InputValueContext>? InputFields => Fields();

        public void Write(TextWriter writer, string indentation)
        {
            Templates.InputObjectTypeGenerator.RenderInputType(this, writer, indentation);
        }

        public IEnumerable<InputValueContext> Fields()
        {
            foreach (var field in declaration.InputValues)
            {
                yield return new InputValueContext(field, options);
            }
        }
    }
}