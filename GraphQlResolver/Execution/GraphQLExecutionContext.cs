using System.Collections.Generic;
using System.Collections.Immutable;
using GraphQLParser.AST;

namespace GraphQlResolver.Execution
{
    public class GraphQLExecutionContext
    {
        public GraphQLDocument Ast { get; }
        public IReadOnlyDictionary<string, string> Arguments { get; }

        public GraphQLExecutionContext(GraphQLDocument ast, IDictionary<string, string> arguments)
        {
            this.Ast = ast;
            Arguments = arguments.ToImmutableDictionary();
        }
    }
}