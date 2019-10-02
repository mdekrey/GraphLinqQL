using GraphLinqQL.Ast.Nodes;
using System;
using System.Linq;

namespace GraphLinqQL.CodeGeneration
{
    public class DefaultTypeResolver : ITypeResolver
    {
        public string Resolve(ITypeNode typeNode, GraphQLGenerationOptions options, Document document, bool nullable = true)
        {
            return GetTypeName(typeNode, options, document, nullable);
        }

        public virtual string GetTypeName(ITypeNode typeNode, GraphQLGenerationOptions options, Document document, bool nullable = true)
        {
            var nullability = nullable && options.ShowNullabilityIndicators() ? "?" : "";
            return typeNode switch
            {
                NonNullType { BaseType: var baseType } => GetTypeName(baseType, options, document, nullable: false),
                ListType { ElementType: var elementType } => GetTypeName(elementType, options, document: document) switch
                {
                    "" => $"IEnumerable{nullability}",
                    var name => $"IEnumerable<{name}>{nullability}"
                },
                TypeName { Name: var name } when options.ScalarTypeMappings.ContainsKey(name) => GetScalarName(options.ScalarTypeMappings[name], options, nullable),
                TypeName { Name: var name } when document != null && document.Children.OfType<UnionTypeDefinition>().Any(u => u.Name == name) => "",
                TypeName { Name: var name } when document != null && document.Children.OfType<EnumTypeDefinition>().Any(u => u.Name == name) => CSharpNaming.GetTypeName(name) + (nullable ? "?" : ""),
                TypeName { Name: var name } => CSharpNaming.GetTypeName(name) + nullability,
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }

        private string GetScalarName(TypeReference targetType, GraphQLGenerationOptions options, bool nullable)
        {
            return targetType.CsharpType + (nullable && (!targetType.CsharpNullable || options.ShowNullabilityIndicators()) ? "?" : "");
        }

        public bool IsNullable(ITypeNode typeNode, GraphQLGenerationOptions options, Document document)
        {
            return typeNode switch
            {
                NonNullType _ => false,
                ListType _ => true,
                //TypeName { Name: var name } when document.Children.OfType<EnumTypeDefinition>().Any(u => u.Name == name) => false,
                //TypeName { Name: var name } when options.ScalarTypeMappings.ContainsKey(name) => options.ScalarTypeMappings[name].CsharpNullable,
                TypeName { Name: var name } => true,
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }
    }
}