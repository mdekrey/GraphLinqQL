using GraphLinqQL.Ast.Nodes;
using GraphLinqQL.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlDirective
    {
        string Name { get; }
        TNode? HandleDirective<TNode>(TNode node, IGraphQlParameterResolver arguments, GraphQLExecutionContext context)
            where TNode : class, INode;
    }
}
