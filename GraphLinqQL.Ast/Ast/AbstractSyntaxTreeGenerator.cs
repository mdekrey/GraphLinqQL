using Antlr4.Runtime;
using GraphLinqQL.Ast.Antlr;
using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast
{
    public class AbstractSyntaxTreeGenerator : IAbstractSyntaxTreeGenerator
    {
        public Document ParseDocument(string text)
        {
            // TODO - it would be better to re-implement this using the new Span types
            var inputStream = new AntlrInputStream(text);
            var lexer = new GraphqlLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GraphqlParser(commonTokenStream);
            var documentContext = parser.document();
            
            var visitor = new GraphQLVisitor();
            return (Document)visitor.Visit(documentContext);
        }
    }
}
