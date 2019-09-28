namespace GraphLinqQL.Ast.Nodes
{
    public class Directive : NodeBase
    {
        public Directive(Location location) : base(location)
        {
        }

        public override NodeKind Kind => NodeKind.Directive;
    }
}