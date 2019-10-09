using Antlr4.Runtime;
using GraphLinqQL.Ast.Antlr;
using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast
{
    public class AbstractSyntaxTreeGenerator : IAbstractSyntaxTreeGenerator
    {
        public Document ParseDocument(string text)
        {
            // TODO - consider an Antlr Lexer engine using Spans
            var inputStream = new AntlrInputStream(text);
            var lexer = new GraphqlLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GraphqlParser(commonTokenStream);
            var documentContext = parser.document();

            AssertNoExceptions(documentContext);
            
            var visitor = new GraphQLVisitor();
            return (Document)visitor.Visit(documentContext);
        }

        private void AssertNoExceptions(ParserRuleContext context)
        {
            var exceptions = FindExceptions(context).ToArray();
            if (exceptions.Length > 1)
            {
                throw new AggregateException("Multiple GraphQL Parse Exceptions found.", exceptions);
            }
            else if (exceptions.Length == 1)
            {
                throw exceptions[0];
            }
        }

        private IEnumerable<GraphqlParseException> FindExceptions(ParserRuleContext context)
        {
            if (context.exception != null)
            {
                yield return new GraphqlParseException($"Unable to parse, could not match {context.GetType().Name} at {context.Start.Line}:{context.Start.Column}", context.Location(), context.exception);
            }
            if (context.children != null)
            {
                foreach (var child in context.children.OfType<ParserRuleContext>())
                {
                    foreach (var ex in FindExceptions(child))
                    {
                        yield return ex;
                    }
                }
            }
        }
    }
}
