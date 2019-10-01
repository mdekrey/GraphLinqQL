using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public interface ITypeDeclaration
    {
        string Label { get; }
        string TypeName { get; }
        void Write(TextWriter writer, string indentation);
    }

    public static class TypeDeclaration
    {
        public static string Declaration(this ITypeDeclaration typeDeclaration, TextWriter writer, string indentation)
        {
            typeDeclaration.Write(writer, indentation);
            return "";
        }
    }
}