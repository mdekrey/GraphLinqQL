namespace GraphLinqQL.Ast.Nodes
{
    public class TypeCondition : NodeBase
    {
        public TypeCondition(TypeName typeName, LocationRange location) : base(location)
        {
            TypeName = typeName;
        }

        public override NodeKind Kind => NodeKind.TypeCondition;

        public TypeName TypeName { get; }
    }
}