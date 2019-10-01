using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public static class TypeDeclaration
    {
        public static string Declaration(this ITypeDeclaration typeDeclaration, TextWriter writer, string indentation)
        {
            typeDeclaration.Write(writer, indentation);
            return "";
        }

        public static string IntrospectionDeclaration(this ITypeDeclaration typeDeclaration, TextWriter writer, string indentation)
        {
            Templates.IntrospectionTypeGenerator.RenderIntrospectionType(typeDeclaration, writer, indentation);
            return "";
        }

        public static string NullabilityIndicator(this ITypeDeclaration model) =>
            model.Options.ShowNullabilityIndicators() ? "?" : "";
    }
}