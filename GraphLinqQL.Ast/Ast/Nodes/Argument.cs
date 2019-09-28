namespace GraphLinqQL.Ast.Nodes
{
    public class Argument : NodeBase
    {
        public Argument(string name, INode value, Location location) : base(location)
        {
            Name = name;
            Value = value;
        }

        public override NodeKind Kind => NodeKind.Argument;

        public string Name { get; }
        public INode Value { get; }
    }
}