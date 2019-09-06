using GraphQLParser.AST;
using GraphQlResolver.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver
{
    public interface IGraphQlDirective
    {
        string Name { get; }
        ASTNode? HandleDirective(ASTNode node, IDictionary<string, object?> arguments, GraphQLExecutionContext context);
    }
}
