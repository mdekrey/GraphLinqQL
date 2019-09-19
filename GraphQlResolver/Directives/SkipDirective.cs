﻿using System;
using System.Collections.Generic;
using System.Text;
using GraphQLParser.AST;
using GraphQlResolver.Execution;

namespace GraphQlResolver.Directives
{
    public class SkipDirective : IGraphQlDirective
    {
        public string Name => "skip";

        public ASTNode? HandleDirective(ASTNode node, IGraphQlParameterResolver arguments, GraphQLExecutionContext context) =>
            arguments.GetParameter<bool>("if") == false ? node : null;

    }
}
