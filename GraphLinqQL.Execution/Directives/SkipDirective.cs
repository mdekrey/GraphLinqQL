using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.Ast.Nodes;
using GraphLinqQL.Execution;

namespace GraphLinqQL.Directives
{
    public class SkipDirective : IGraphQlDirective
    {
        public string Name => "skip";

        public TNode? HandleDirective<TNode>(TNode node, IGraphQlParameterResolver arguments, FieldContext fieldContext, GraphQLExecutionContext context)
            where TNode : class, INode =>
            arguments.GetParameter<bool>("if", fieldContext) == false ? node : null;
    }
}
