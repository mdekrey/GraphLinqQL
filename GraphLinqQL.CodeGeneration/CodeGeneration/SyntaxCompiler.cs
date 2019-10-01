using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using GraphLinqQL.Ast.Nodes;
using GraphLinqQL.CodeGeneration.Templates;

namespace GraphLinqQL.CodeGeneration
{
    internal class SyntaxCompiler
    {
        internal static CompileResult Compile(Document document, string filename, GraphQLGenerationOptions options)
        {
            using var writer = new StringWriter();
            var context = new DocumentContext(document, filename, options);
            GenerateGraphQLSchemaCode.RenderDocument(context, writer);

            return new CompileResult(context.CompilerErrors, writer.ToString());
        }
    }
}