using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.CodeGeneration
{
    public static class ResolverExtensions
    {
        public static string Resolve(this GraphQLGenerationOptions options, ITypeNode typeNode, Document document, bool nullable = true)
        {
            return options.TypeResolver.Resolve(typeNode, options, document, nullable: nullable);
        }

        public static string Resolve(this GraphQLGenerationOptions options, IValueNode value, ITypeNode typeNode, Document document)
        {
            return options.ValueResolver.Resolve(value, typeNode, options, document);
        }

        public static string ResolveJson(this GraphQLGenerationOptions options, IValueNode value, ITypeNode typeNode, Document document)
        {
            return options.ValueResolver.ResolveJson(value, typeNode, options, document);
        }

        public static Directive? FindObsoleteDirective(this IReadOnlyList<Directive> directives)
        {
            return directives.FirstOrDefault(d => d.Name == "Obsolete")!;
        }

        public static string? ObsoleteReason(this Directive obsoleteDirective, GraphQLGenerationOptions options, Document document)
        {
            var reason = obsoleteDirective.Arguments.FirstOrDefault(a => a.Name == "reason")?.Value;
            if (reason != null)
            {
                return options.Resolve(reason, new TypeName("String", new LocationRange()), document);
            }
            return null;
        }


        public static string ResolveIntrospection(this GraphQLGenerationOptions options, ITypeNode typeNode)
        {
            return typeNode switch
            {
                NonNullType { BaseType: var baseType } => $"global::GraphLinqQL.Introspection.NonNullTypeInformation<{options.ResolveIntrospection(baseType)}>",
                ListType { ElementType: var elementType } => $"global::GraphLinqQL.Introspection.ListTypeInformation<{options.ResolveIntrospection(elementType)}>",
                TypeName { Name: var name } => $"Introspection.{CSharpNaming.GetTypeName(name)}",
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }
    }
}
