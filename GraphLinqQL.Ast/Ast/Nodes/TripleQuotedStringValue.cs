using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class TripleQuotedStringValue : NodeBase, IValueNode, IStringValue
    {
        public TripleQuotedStringValue(string quotedStringValue, LocationRange location) : base(location)
        {
            QuotedStringValue = quotedStringValue;
        }

        public override NodeKind Kind => NodeKind.TripleQuotedStringValue;

        public string QuotedStringValue { get; }

        public string Text => QuotedStringValue.Substring(3, QuotedStringValue.Length - 6).Replace("\\\"\"\"", "\"\"\"");

        public TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context)
        {
            return converter.VisitString((IStringValue)this, context);
        }
    }
}
