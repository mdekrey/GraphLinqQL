namespace GraphLinqQL.Ast.Nodes
{
    public class IntValue : NodeBase, IValueNode
    {
        public IntValue(string tokenValue, Location location) : base(location)
        {
            TokenValue = tokenValue;
        }

        public override NodeKind Kind => NodeKind.IntValue;

        public string TokenValue { get; }
    }
}
