using System.Collections.Generic;
using System.Collections.Immutable;
using GraphQLParser.AST;

namespace GraphQlResolver.Execution
{
    public class GraphQLExecutionContext
    {
        public GraphQLDocument Ast { get; }
        public IReadOnlyDictionary<string, object?> Arguments { get; }

        public GraphQLExecutionContext(GraphQLDocument ast, IDictionary<string, object?> arguments)
        {
            this.Ast = ast;
            Arguments = arguments.ToImmutableDictionary();
        }
    }
}