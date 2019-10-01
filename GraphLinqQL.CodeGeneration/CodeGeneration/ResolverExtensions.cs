using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.CodeGeneration
{
    public static class ResolverExtensions
    {
        public static string Resolve(this GraphQLGenerationOptions options, ITypeNode typeNode, bool nullable = true, Document? document = null)
        {
            return options.TypeResolver.Resolve(typeNode, options, nullable, document);
        }

        public static string Resolve(this GraphQLGenerationOptions options, IValueNode value, ITypeNode typeNode)
        {
            return options.ValueResolver.Resolve(value, typeNode, options);
        }

        public static Directive? FindObsoleteDirective(this IReadOnlyList<Directive> directives)
        {
            return directives.FirstOrDefault(d => d.Name == "Obsolete")!;
        }

        public static string? ObsoleteReason(this Directive obsoleteDirective, GraphQLGenerationOptions options)
        {
            var reason = obsoleteDirective.Arguments.FirstOrDefault(a => a.Name == "reason")?.Value;
            if (reason != null)
            {
                return options.Resolve(reason, new TypeName("String", new LocationRange()));
            }
            return null;
        }


        public static string ResolveIntrospection(this GraphQLGenerationOptions options, ITypeNode typeNode)
        {
            return typeNode switch
            {
                NonNullType { BaseType: var baseType } => $"NonNullTypeInformation<{options.ResolveIntrospection(baseType)}>",
                ListType { ElementType: var elementType } => $"ListTypeInformation<{options.ResolveIntrospection(elementType)}>",
                TypeName { Name: var name } => $"Introspection.{CSharpNaming.GetTypeName(name)}",
                _ => throw new InvalidOperationException($"Expected known type node, got {typeNode.GetType().FullName}"),
            };
        }
    }
}
