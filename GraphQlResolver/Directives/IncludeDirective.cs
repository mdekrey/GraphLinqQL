﻿using System;
using System.Collections.Generic;
using System.Text;
using GraphQLParser.AST;
using GraphQlResolver.Execution;

namespace GraphQlResolver.Directives
{
    public class IncludeDirective : IGraphQlDirective
    {
        public string Name => "include";

        public ASTNode? HandleDirective(ASTNode node, IGraphQlParameterResolver arguments, GraphQLExecutionContext context) =>
            arguments.GetParameter<bool>("if") == true ? node : null;
    }
}
