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
        TNode? HandleDirective<TNode>(TNode node, IGraphQlParameterResolver arguments, FieldContext fieldContext, GraphQLExecutionContext context)
            where TNode : class, INode;
    }
}
