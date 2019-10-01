using GraphLinqQL.Ast.Nodes;
using System;
using System.Linq;

namespace GraphLinqQL.CodeGeneration
{
    public class DefaultTypeResolver : ITypeResolver
    {
        public string Resolve(ITypeNode typeNode, GraphQLGenerationOptions options)
        {
            return GetTypeName(typeNode, options);
        }
        public string ResolveNonNull(ITypeNode typeNode, GraphQLGenerationOptions options)
        {
            return GetTypeName(typeNode, options, nullable: false);
        }

        public virtual string GetTypeName(ITypeNode typeNode, GraphQLGenerationOptions options, bool nullable = true)
        {
            return typeNode switch
            {
                NonNullType { BaseType: var baseType } => GetTypeName(baseType, options, false),
                ListType { ElementType: var elementType } => $"IEnumerable<{GetTypeName(elementType, options)}>{(nullable && options.ShowNullabilityIndicators() ? "?" : "")}",
                TypeName { Name: var name } => options.ScalarTypes.FirstOrDefault(s => s.GraphQLType == name) switch
                    {
                        null when name == "ID" => "string",
                        null => CSharpNaming.GetTypeName(name) + (nullable && options.ShowNullabilityIndicators() ? "?" : ""),
                        var scalar => GetScalarName(scalar, options, nullable),
                    },
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
                TypeName { Name: var name } => options.ScalarTypes.FirstOrDefault(s => s.GraphQLType == name) switch
                {
                    null when name == "ID" => true /* string */,
                    null => true,
                    var scalar => scalar.CsharpNullable,
                },
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }
    }
}