using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.Execution;
using GraphLinqQL.Ast.Nodes;
using GraphLinqQL.Resolution;

namespace GraphLinqQL.Directives
{
    public class IncludeDirective : IGraphQlDirective
    {
        public string Name => "include";

        public TNode? HandleDirective<TNode>(TNode node, IGraphQlParameterResolver arguments, GraphQLExecutionContext context)
            where TNode : class, INode =>
            arguments.GetParameter<bool>("if") == true ? node : null;
    }
}
