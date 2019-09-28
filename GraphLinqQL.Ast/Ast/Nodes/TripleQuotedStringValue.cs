namespace GraphLinqQL.Ast.Nodes
{
    public class TripleQuotedStringValue : NodeBase, IValueNode
    {
        public TripleQuotedStringValue(string quotedStringValue, LocationRange location) : base(location)
        {
            QuotedStringValue = quotedStringValue;
        }

        public override NodeKind Kind => NodeKind.TripleQuotedStringValue;

        public string QuotedStringValue { get; }
    }
}
