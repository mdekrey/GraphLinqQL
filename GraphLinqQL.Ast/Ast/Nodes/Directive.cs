namespace GraphLinqQL.Ast.Nodes
{
    public class Directive : NodeBase
    {
        public Directive(LocationRange location) : base(location)
        {
        }

        public override NodeKind Kind => NodeKind.Directive;
    }
}