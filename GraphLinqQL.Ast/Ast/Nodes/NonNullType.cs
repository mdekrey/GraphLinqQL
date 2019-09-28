namespace GraphLinqQL.Ast.Nodes
{
    public class NonNullType : NodeBase, ITypeNode
    {
        public NonNullType(ITypeNode baseType, LocationRange location) : base(location)
        {
            BaseType = baseType;
        }

        public override NodeKind Kind => NodeKind.NonNullType;

        public ITypeNode BaseType { get; }
    }
}
