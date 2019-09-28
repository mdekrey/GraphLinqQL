namespace GraphLinqQL.Ast.Nodes
{
    public class VariableDefinition : NodeBase
    {
        public VariableDefinition(Variable variable, ITypeNode graphQLType, IValueNode? defaultValue, LocationRange location)
            : base(location)
        {
            this.Variable = variable;
            this.GraphQLType = graphQLType;
            this.DefaultValue = defaultValue;
        }

        public override NodeKind Kind => NodeKind.VariableDefinition;
        public Variable Variable { get; }
        public ITypeNode GraphQLType { get; }
        public IValueNode? DefaultValue { get; }
    }
}