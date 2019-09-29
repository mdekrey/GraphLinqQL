using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL.Execution
{
    public class GraphQLExecutionContext
    {
        public Document Ast { get; }
        public IDictionary<string, IGraphQlParameterInfo> Arguments { get; }

        public GraphQLExecutionContext(Document ast, IDictionary<string, IGraphQlParameterInfo> arguments)
        {
            this.Ast = ast;
            Arguments = arguments.ToImmutableDictionary();
        }
    }
}