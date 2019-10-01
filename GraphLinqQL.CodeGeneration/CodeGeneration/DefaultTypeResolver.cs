using GraphLinqQL.Ast.Nodes;
using System;
using System.Linq;

namespace GraphLinqQL.CodeGeneration
{
    public class DefaultTypeResolver : ITypeResolver
    {
        public string Resolve(ITypeNode typeNode, GraphQLGenerationOptions options, bool nullable = true, Document? document = null)
        {
            return GetTypeName(typeNode, options, nullable, document);
        }

        public virtual string GetTypeName(ITypeNode typeNode, GraphQLGenerationOptions options, bool nullable = true, Document? document = null)
        {
            var nullability = nullable && options.ShowNullabilityIndicators() ? "?" : "";
            return typeNode switch
            {
                NonNullType { BaseType: var baseType } => GetTypeName(baseType, options, false, document: document),
                ListType { ElementType: var elementType } => GetTypeName(elementType, options, document: document) switch
                {
                    "" => $"IEnumerable{nullability}",
                    var name => $"IEnumerable<{name}>{nullability}"
                },
                TypeName { Name: var name } when options.ScalarTypeMappings.ContainsKey(name) => GetScalarName(options.ScalarTypeMappings[name], options, nullable),
                TypeName { Name: var name } when document != null && document.Children.OfType<UnionTypeDefinition>().Any(u => u.Name == name) => "",
                TypeName { Name: var name } => CSharpNaming.GetTypeName(name) + nullability,
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }

        private string GetScalarName(TypeReference targetType, GraphQLGenerationOptions options, bool nullable)
        {
            return targetType.CsharpType + (nullable && (!targetType.CsharpNullable || options.ShowNullabilityIndicators()) ? "?" : "");
        }

        public bool IsNullable(ITypeNode typeNode, GraphQLGenerationOptions options)
        {
            return typeNode switch
            {
                NonNullType _ => false,
                ListType _ => true,
                TypeName { Name: var name } => options.ScalarTypeMappings.ContainsKey(name)
                    ? options.ScalarTypeMappings[name].CsharpNullable
                    : true,
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }
    }
}