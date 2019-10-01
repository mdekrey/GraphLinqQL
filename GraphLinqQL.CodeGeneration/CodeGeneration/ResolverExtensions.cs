using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.CodeGeneration
{
    public static class ResolverExtensions
    {
        public static string Resolve(this GraphQLGenerationOptions options, ITypeNode typeNode, bool nullable = true)
        {
            if (nullable)
                return options.TypeResolver.Resolve(typeNode, options);
            else 
                return options.TypeResolver.ResolveNonNull(typeNode, options);
        }

        public static string Resolve(this GraphQLGenerationOptions options, IValueNode value, ITypeNode typeNode)
        {
            return options.ValueResolver.Resolve(value, typeNode, options);
        }
    }
}
