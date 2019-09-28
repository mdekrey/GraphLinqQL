namespace GraphLinqQL.Ast.Nodes
{
    public class VariableDefinition : NodeBase
    {
        public VariableDefinition(Variable variable, ITypeNode graphQLType, IValueNode? defaultValue, Location location)
            : base(location)
        {
            this.Variable = variable;
            this.GraphQLType = graphQLType;

        }

        public override NodeKind Kind => NodeKind.VariableDefinition;
        public Variable Variable { get; }
        public ITypeNode GraphQLType { get; }
        public IValueNode? DefaultValueJson { get; }
    }
}