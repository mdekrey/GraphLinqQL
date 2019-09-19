using GraphQLParser.AST;
using GraphLinqQL.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlDirective
    {
        string Name { get; }
        ASTNode? HandleDirective(ASTNode node, IGraphQlParameterResolver arguments, GraphQLExecutionContext context);
    }
}
