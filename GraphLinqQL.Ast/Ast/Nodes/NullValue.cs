namespace GraphLinqQL.Ast.Nodes
{
    public class NullValue : NodeBase, IValueNode
    {
        public NullValue(Location location) : base(location)
        {
        }

        public override NodeKind Kind => NodeKind.NullValue;
    }
}
