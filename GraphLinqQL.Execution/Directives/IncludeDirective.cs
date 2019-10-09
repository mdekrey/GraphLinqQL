using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.Execution;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.Directives
{
    public class IncludeDirective : IGraphQlDirective
    {
        public string Name => "include";

        public TNode? HandleDirective<TNode>(TNode node, IGraphQlParameterResolver arguments, FieldContext fieldContext, GraphQLExecutionContext context)
            where TNode : class, INode =>
            arguments.GetParameter<bool>("if", fieldContext) == true ? node : null;
    }
}
