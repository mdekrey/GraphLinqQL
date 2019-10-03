using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using GraphLinqQL.Ast.Nodes;
#pragma warning disable CA1307 // Specify StringComparison

namespace GraphLinqQL.CodeGeneration
{
    internal class DefaultValueResolver : IValueResolver
    {
        private readonly DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));

        public virtual string Resolve(IValueNode value, ITypeNode typeNode, GraphQLGenerationOptions options, Document document)
        {
            return value switch
            {
                ArrayValue array when typeNode is ListType list =>
                    $"new {options.Resolve(list.ElementType, document)} [] {{ {string.Join(", ", array.Values.Select(v => Resolve(v, list.ElementType, options, document)))} }}",
                BooleanValue booleanValue =>
                    booleanValue.TokenValue == true ? "true" : "false",
                EnumValue enumValue when typeNode is TypeName typeName =>
                    $"{options.Namespace}.{options.Resolve(typeName, document, nullable: false)}.{enumValue.TokenValue}",
                FloatValue floatValue =>
                    floatValue.TokenValue,
                IntValue intValue =>
                    intValue.TokenValue,
                NullValue _ => "null",
                ObjectValue objectValue when typeNode is TypeName typeName => RenderObjectInstantiation(objectValue, typeName, options, document),
                StringValue stringValue => $@"@""{stringValue.Text.Replace("\"", "\"\"")}""",
                TripleQuotedStringValue stringValue => $@"@""{stringValue.Text.Replace("\"", "\"\"")}""",
                _ => throw new InvalidOperationException($"Cannot render a C# representation for the default value of type {value.Kind.ToString("g")} for type {options.Resolve(typeNode, document, nullable: false)}"),
            };
        }

        private string RenderObjectInstantiation(ObjectValue objectValue, TypeName typeName, GraphQLGenerationOptions options, Document document)
        {
            var typeDefinition = document.Children.OfType<InputObjectTypeDefinition>().Single(def => def.Name == typeName.Name);
            string GetValue(KeyValuePair<string, IValueNode> field)
            {
                return Resolve(field.Value, typeDefinition.InputValues.First(iv => iv.Name == field.Key).TypeNode, options, document);
            }
            return $@"new {options.Resolve(typeName, document, nullable: false)}
                    {{
                        {objectValue.Fields.Select(field => @$"{CSharpNaming.GetPropertyName(field.Key)} = {GetValue(field)}, ")}
                    }}";
        }

        public virtual string ResolveJson(IValueNode value, ITypeNode typeNode, GraphQLGenerationOptions options, Document document)
        {
            return JsonEncode(InternalResolveJson(value, typeNode, options, document));
        }

        public virtual string InternalResolveJson(IValueNode value, ITypeNode typeNode, GraphQLGenerationOptions options, Document document)
        {
            return value switch
            {
                ArrayValue array when typeNode is ListType list =>
                    $"[ {string.Join(", ", array.Values.Select(v => Resolve(v, list.ElementType, options, document)))} ]",
                BooleanValue booleanValue =>
                    booleanValue.TokenValue == true ? "true" : "false",
                EnumValue enumValue when typeNode is TypeName typeName =>
                    $"\"{enumValue.TokenValue}\"",
                FloatValue floatValue =>
                    floatValue.TokenValue,
                IntValue intValue =>
                    intValue.TokenValue,
                NullValue _ => "null",
                ObjectValue objectValue when typeNode is TypeName typeName => RenderJsonObject(objectValue, typeName, options, document),
                IStringValue stringValue => JsonEncode(stringValue.Text),
                _ => throw new InvalidOperationException($"Cannot render a JSON representation for the default value of type {value.Kind.ToString("g")} for type {options.Resolve(typeNode, document, nullable: false)}"),
            };
        }

        private string RenderJsonObject(ObjectValue objectValue, TypeName typeName, GraphQLGenerationOptions options, Document document)
        {
            var typeDefinition = document.Children.OfType<InputObjectTypeDefinition>().Single(def => def.Name == typeName.Name);
            string GetValue(KeyValuePair<string, IValueNode> field)
            {
                return InternalResolveJson(field.Value, typeDefinition.InputValues.First(iv => iv.Name == field.Key).TypeNode, options, document);
            }
            return $@"{{
                        {objectValue.Fields.Select(field => $"\"{CSharpNaming.GetPropertyName(field.Key)}\" {GetValue(field)},")}
                    }}";
        }

        private string JsonEncode(string text)
        {
            // Create a stream to serialize the object to.  
            using var ms = new MemoryStream();

            // Serializer the User object to the stream.  
            serializer.WriteObject(ms, text);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}