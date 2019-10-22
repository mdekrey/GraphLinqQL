using GraphLinqQL.Resolution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphLinqQL
{
    class RoslynServices
    {
        internal static Diagnostic[] Compile(string code, float languageVersion)
        {
            var sourceLanguage = new CSharpLanguage();
            var syntaxTree = sourceLanguage.ParseText(code, SourceCodeKind.Regular, languageVersion);
            var syntaxDiagnostics = syntaxTree.GetDiagnostics().ToArray();
            if (syntaxDiagnostics.Any())
            {
                return syntaxDiagnostics;
            }
            var compilation = sourceLanguage.CreateLibraryCompilation("test", false);
            var compilationDiagonstics = compilation.GetDiagnostics().ToArray();
            return compilationDiagonstics;
        }


    }

    public interface ILanguageService
    {
        SyntaxTree ParseText(string code, SourceCodeKind kind, float languageVersion);

        Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations);
    }

    public class CSharpLanguage : ILanguageService
    {
        private readonly IReadOnlyCollection<MetadataReference> _references = new[] {
          MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(ValueTuple<>).GetTypeInfo().Assembly.Location),
          MetadataReference.CreateFromFile(typeof(IGraphQlResolvable).GetTypeInfo().Assembly.Location),
        };

        public SyntaxTree ParseText(string sourceCode, SourceCodeKind kind, float languageVersion)
        {
            var languageVersionEnum = ToLanguageVersion(languageVersion);
            var options = new CSharpParseOptions(kind: kind, languageVersion: languageVersionEnum);

            // Return a syntax tree of our source code
            return CSharpSyntaxTree.ParseText(sourceCode, options);
        }

        private LanguageVersion ToLanguageVersion(float languageVersion)
        {
            var expectedValue =
                languageVersion <= 7f ? (LanguageVersion)(int)languageVersion
                : (LanguageVersion)(int)(languageVersion * 100);
            if (Enum.GetValues(typeof(LanguageVersion)).Cast<LanguageVersion>().Contains(expectedValue))
            {
                return expectedValue;
            }
            return LanguageVersion.Latest;
        }

        public Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations)
        {
            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: enableOptimisations ? OptimizationLevel.Release : OptimizationLevel.Debug,
                allowUnsafe: true);

            return CSharpCompilation.Create(assemblyName, options: options, references: _references);
        }
    }
}