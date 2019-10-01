using System;
using System.Linq;
using GraphLinqQL.Ast.Nodes;
#pragma warning disable CA1307 // Specify StringComparison

namespace GraphLinqQL.CodeGeneration
{
    internal class DefaultValueResolver : IValueResolver
    {
        public string Resolve(IValueNode reason, ITypeNode typeNode, GraphQLGenerationOptions options)
        {
            return ResolveValue(reason, typeNode, options);
        }

        public virtual string ResolveValue(IValueNode reason, ITypeNode typeNode, GraphQLGenerationOptions options)
        {
            
            return reason switch
            {
                ArrayValue array when typeNode is ListType list =>
                    $"new {options.Resolve(list.ElementType)} [] {{ {string.Join(", ", array.Values.Select(v => ResolveValue(v, list.ElementType, options)))} }}",
                BooleanValue booleanValue =>
                    booleanValue.TokenValue == true ? "true" : "false",
                EnumValue enumValue when typeNode is TypeName typeName =>
                    $"{options.Resolve(typeName)}.${enumValue.TokenValue}",
                FloatValue floatValue =>
                    floatValue.TokenValue,
                IntValue intValue =>
                    intValue.TokenValue,
                NullValue _ => "null",
                ObjectValue objectValue => throw new NotImplementedException("Unable to cross-reference types with schema"), /* TODO */
                StringValue stringValue => $@"@""{stringValue.Text.Replace("\"", "\"\"")}""",
                TripleQuotedStringValue stringValue => $@"@""{stringValue.Text.Replace("\"", "\"\"")}""",
                _ => "null /* TODO - add warning */"
            };
        }
    }
}