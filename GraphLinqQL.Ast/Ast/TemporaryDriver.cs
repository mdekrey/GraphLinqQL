using Antlr4.Runtime;
using GraphLinqQL.Ast.Antlr;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast
{
    public static class TemporaryDriver
    {
        public static void Parse(string text)
        {
            var inputStream = new AntlrInputStream(text);
            var speakLexer = new GraphqlLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(speakLexer);
            var speakParser = new GraphqlParser(commonTokenStream);
            var documentContext = speakParser.document();
            var visitor = new GraphQLVisitor();
            var result = visitor.Visit(documentContext);
        }
    }
}
