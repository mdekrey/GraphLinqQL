using Antlr4.Runtime;
using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Antlr
{
    public static class AntlrExtensions
    {
        public static Location Location(this ParserRuleContext ruleContext) => new Location(ruleContext.Start, ruleContext.Stop);
    }
}
