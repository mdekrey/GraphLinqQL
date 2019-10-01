using System;
using System.Collections.Generic;
using System.IO;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct ScalarTypeContext : ITypeDeclaration
    {
        private readonly ScalarTypeDefinition definition;
        private readonly GraphQLGenerationOptions options;

        public ScalarTypeContext(ScalarTypeDefinition definition, GraphQLGenerationOptions options)
        {
            this.definition = definition;
            this.options = options;
        }

        public GraphQLGenerationOptions Options => options;

        public string Label => definition.Name;
        public string TypeName => CSharpNaming.GetTypeName(definition.Name);
        public string MappedTypeName => options.ScalarTypeMappings[definition.Name] switch
        {
            null => throw new InvalidOperationException($"Type {definition.Name} did not have a C# mapping"),
            var value => $"{value.CsharpType}{(value.CsharpNullable ? "" : "?")}",
        };

        public string? Description => definition.Description;

        public string TypeKind => "Scalar";

        public IEnumerable<string>? ImplementedInterfaces => null;

        public IEnumerable<string>? PossibleTypes => null;

        public IEnumerable<ObjectFieldContext>? Fields => null;

        public IEnumerable<EnumValueContext>? EnumValues => null;

        public IEnumerable<InputObjectFieldContext>? InputFields => null;

        public void Write(TextWriter writer, string indentation)
        {
            writer.WriteLine($"// Scalar: {Label}");
        }
    }
}