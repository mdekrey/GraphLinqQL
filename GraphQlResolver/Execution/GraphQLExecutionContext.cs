using System.Collections.Generic;
using GraphQLParser.AST;

namespace GraphQlResolver.Execution
{
    internal class GraphQLExecutionContext
    {
        public GraphQLDocument Ast { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

        public GraphQLExecutionContext(GraphQLDocument ast, IDictionary<string, object> arguments)
        {
            this.Ast = ast;
            this.Arguments = arguments;
        }
    }
}