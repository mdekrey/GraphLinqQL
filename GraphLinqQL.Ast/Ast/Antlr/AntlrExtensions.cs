using Antlr4.Runtime;
using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Antlr
{
    public static class AntlrExtensions
    {
        public static LocationRange Location(this ParserRuleContext ruleContext) => new LocationRange(ruleContext.Start.Location(), ruleContext.Stop.Location());

        public static Location Location(this IToken token) => new Location(token.Line, token.Column);
    }
}
