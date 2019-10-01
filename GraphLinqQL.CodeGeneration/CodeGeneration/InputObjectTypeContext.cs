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

        public string Label => declaration.Name;
        public string TypeName => CSharpNaming.GetTypeName(declaration.Name);

        public string? Description => declaration.Description;

        public void Write(TextWriter writer, string indentation)
        {
            Templates.InputObjectTypeGenerator.RenderInputType(this, writer, indentation);
        }

        public IEnumerable<InputObjectFieldContext> Fields()
        {
            foreach (var field in declaration.InputValues)
            {
                yield return new InputObjectFieldContext(field, options);
            }
        }
    }
}