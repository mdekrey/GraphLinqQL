using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CA1308 // Normalize strings to uppercase

namespace GraphLinqQL.CodeGeneration
{
    public static class CSharpNaming
    {
        public static string GetTypeName(string name) =>
            ToPascalCase(FromUnknownStyle(name));

        internal static string GetPropertyName(string name) =>
            ToPascalCase(FromUnknownStyle(name)); // TODO - guard this C# name

        internal static string GetFieldName(string name) =>
            ToMedialCapitals(FromUnknownStyle(name)); // TODO - guard this C# name


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


    }
}