using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CA1308 // Normalize strings to uppercase

namespace GraphLinqQL.CodeGeneration
{
    public static class CSharpNaming
    {
        private static IReadOnlyList<string> CSharpKeywords = new[]
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "clas",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "using",
            "static",
            "virtual",
            "void",
            "volatile",
            "while",
        };

        public static string GetTypeName(string name) =>
            ToPascalCase(FromUnknownStyle(name));

        internal static string GetFieldName(string name)
        {
            var result = ToMedialCapitals(FromUnknownStyle(name));
            if (CSharpKeywords.Contains(result))
            {
                return "_" + result;
            }
            return result;
        }


        private static string ToMedialCapitals(string[] words) =>
            string.Join("", words.Select((word, index) => index == 0 ? word.ToLowerInvariant() : word.Substring(0, 1).ToUpperInvariant() + word.Substring(1).ToLowerInvariant()));

        private static string ToPascalCase(string[] words) =>
            string.Join("", words.Select(word => word.Substring(0, 1).ToUpperInvariant() + word.Substring(1).ToLowerInvariant()));

        private static readonly Regex Pattern = new Regex("((?<first>^_+)|_+|(?<first>[a-z0-9])(?<second>[A-Z])|(?<first>[a-zA-Z])(?<second>[0-9]))", RegexOptions.Compiled);
        private static string[] FromUnknownStyle(string name)
        {
            return Pattern.Replace(name, match =>
            {
                return match.Groups["first"].Value + " " + match.Groups["second"].Value;
            }).Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        public static string GetPropertyName(this Ast.Nodes.EnumTypeDefinition definition, string name)
        {
            var original = definition.EnumValues.Select(ev => ev.EnumValue.TokenValue);
            return ConvertIfSafe(name, original);
        }

        private static string ConvertIfSafe(string name, IEnumerable<string> names, string? disallowed = null)
        {
            Func<string, string> normalize = Convert;
            var normalized = names.Select(v => Convert(v)).ToArray();
            if (normalized.Length != normalized.Distinct().Count())
            {
                normalize = _ => _;
            }
            normalized = names.Where(n => n != name).ToArray();
            var resultName = normalize(name);
            while (normalized.Contains(resultName) || disallowed == resultName)
            {
                resultName = "_" + resultName;
            }
            return resultName;
            string Convert(string name) => ToPascalCase(FromUnknownStyle(name));
        }

        public static string GetPropertyName(this Ast.Nodes.FieldDefinition definition, string name)
        {
            var original = definition.Arguments.Select(ev => ev.Name);
            return ConvertIfSafe(name, original);
        }

        public static string GetPropertyName(this Ast.Nodes.ObjectTypeDefinition definition, string name)
        {
            var original = definition.Fields.Select(ev => ev.Name);
            return ConvertIfSafe(name, original, definition.Name);
        }

        public static string GetPropertyName(this Ast.Nodes.InputObjectTypeDefinition definition, string name)
        {
            var original = definition.InputValues.Select(ev => ev.Name);
            return ConvertIfSafe(name, original, definition.Name);
        }

        public static string GetPropertyName(this Ast.Nodes.DirectiveDefinition definition, string name)
        {
            var original = definition.Arguments.Select(ev => ev.Name);
            return ConvertIfSafe(name, original);
        }

        public static string GetPropertyName(this Ast.Nodes.InterfaceTypeDefinition definition, string name)
        {
            var original = definition.Fields.Select(ev => ev.Name);
            return ConvertIfSafe(name, original);
        }
    }
}