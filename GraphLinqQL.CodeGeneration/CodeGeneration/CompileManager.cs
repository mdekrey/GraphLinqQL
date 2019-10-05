using GraphLinqQL.Ast;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GraphLinqQL.CodeGeneration
{
    public static class CompileManager
    {
        public static void CompileFile(string inputFile, string outputFile, Action<CompilerError> logError, GraphQLGenerationOptions options)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException(nameof(inputFile));
            }

            if (logError == null)
            {
                throw new ArgumentNullException(nameof(logError));
            }

            outputFile ??= inputFile + ".g.cs";

            var subject = File.ReadAllText(inputFile);
            var result = CompileString(subject, fileName: MakePragmaPath(inputFile, outputFile), options: options);

            var hadFatal = false;
            foreach (var error in result.Errors)
            {
                hadFatal |= !error.IsWarning;
                logError(error);
            }

            if (!hadFatal)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                File.WriteAllText(outputFile, result.Code);
            }
            else if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }

        public static CompileResult CompileString(string subject, string fileName, GraphQLGenerationOptions options)
        {
            var document = new AbstractSyntaxTreeGenerator().ParseDocument(subject ?? string.Empty);

            return SyntaxCompiler.Compile(document, fileName, options);
        }


        /// <summary>
        /// Compares the input and output path and returns the appropriate filename to use in <c>#line</c> pragmas.
        /// </summary>
        /// <param name="input">The input file path.</param>
        /// <param name="output">The output file path.</param>
        /// <returns>The input path transformed to the appropriate pragma path.</returns>
        public static string MakePragmaPath(string input, string output)
        {
            output = Path.GetFullPath(output);
            input = Path.GetFullPath(input);
            var relativeUri = new Uri(output).MakeRelativeUri(new Uri(input));
            return relativeUri.ToString().IndexOf('/') != -1
                ? input
                : Path.GetFileName(input);
        }
    }
}
