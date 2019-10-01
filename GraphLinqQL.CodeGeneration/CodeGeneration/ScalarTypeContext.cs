using System;
using System.IO;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct ScalarTypeContext : ITypeDeclaration
    {
        public ScalarTypeContext(ScalarTypeDefinition scalarTypeDefinition)
            : this(scalarTypeDefinition.Name, null!)
        {
        }

        public ScalarTypeContext(string label, string typeName)
        {
            Label = label;
            TypeName = typeName;
        }

        public string Label { get; }
        public string TypeName { get; }

        public void Write(TextWriter writer, string indentation)
        {
            writer.WriteLine($"// Scalar: {Label}");
        }
    }
}