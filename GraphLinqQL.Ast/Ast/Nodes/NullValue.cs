namespace GraphLinqQL.Ast.Nodes
{
    public class NullValue : NodeBase, IValueNode
    {
        public NullValue(LocationRange location) : base(location)
        {
        }

        public override NodeKind Kind => NodeKind.NullValue;
    }
}
