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

        public TNode? HandleDirective<TNode>(TNode node, IGraphQlParameterResolver arguments, GraphQLExecutionContext context)
            where TNode : class, INode =>
            arguments.GetParameter<bool>("if") == false ? node : null;
    }
}
