using System;
using System.Collections.Generic;
using System.Text;
using GraphQLParser.AST;
using GraphQlResolver.Execution;

namespace GraphQlResolver.Directives
{
    public class IncludeDirective : IGraphQlDirective
    {
        public string Name => "include";

        public ASTNode? HandleDirective(ASTNode node, IDictionary<string, object?> arguments, GraphQLExecutionContext context) =>
            Convert.ToBoolean(arguments["if"]) == true ? node : null;
    }
}
