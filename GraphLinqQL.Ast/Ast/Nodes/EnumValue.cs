namespace GraphLinqQL.Ast.Nodes
{
    public class EnumValue : NodeBase, IValueNode
    {
        public EnumValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue;
        }

        public override NodeKind Kind => NodeKind.EnumValue;

        public string TokenValue { get; }
    }
}
