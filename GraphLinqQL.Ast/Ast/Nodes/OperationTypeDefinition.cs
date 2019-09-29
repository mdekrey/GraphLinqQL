namespace GraphLinqQL.Ast.Nodes
{
    public class OperationTypeDefinition : NodeBase
    {
        public OperationTypeDefinition(OperationType operationType, TypeName typeName, LocationRange location) : base(location)
        {
            OperationType = operationType;
            TypeName = typeName;
        }

        public override NodeKind Kind => NodeKind.OperationTypeDefinition;

        public OperationType OperationType { get; }
        public TypeName TypeName { get; }
    }
}