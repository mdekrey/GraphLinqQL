using System.Collections.Generic;
using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public interface ITypeDeclaration
    {
        GraphQLGenerationOptions Options { get; }
        string Label { get; }
        string TypeName { get; }
        string? Description { get; }
        string TypeKind { get; }
        IEnumerable<string>? ImplementedInterfaces { get; }
        IEnumerable<string>? PossibleTypes { get; }
        IEnumerable<ObjectFieldContext>? Fields { get; }
        IEnumerable<EnumValueContext>? EnumValues { get; }
        IEnumerable<InputObjectFieldContext>? InputFields { get; }
        void Write(TextWriter writer, string indentation);
    }
}