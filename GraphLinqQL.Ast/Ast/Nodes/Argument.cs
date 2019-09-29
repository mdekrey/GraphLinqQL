namespace GraphLinqQL.Ast.Nodes
{
    public class Argument : NodeBase
    {
        public Argument(string name, IValueNode value, LocationRange location) : base(location)
        {
            Name = name;
            Value = value;
        }

        public override NodeKind Kind => NodeKind.Argument;

        public string Name { get; }
        public IValueNode Value { get; }
    }
}