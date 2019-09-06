using System;
using System.Collections.Generic;
using System.Text;
using GraphQLParser.AST;
using GraphQlResolver.Execution;

namespace GraphQlResolver.Directives
{
    public class SkipDirective : IGraphQlDirective
    {
        public string Name => "skip";

        public ASTNode? HandleDirective(ASTNode node, IDictionary<string, object?> arguments, GraphQLExecutionContext context) =>
            Convert.ToBoolean(arguments["if"]) == false ? node : null;
    }
}
