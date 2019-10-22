using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlScalarResult<string?> DefaultValue() =>
            this.Resolve(field => field.DefaultValue);

        public override IGraphQlScalarResult<string?> Description() =>
            this.Resolve(field => field.Description);

        public override IGraphQlScalarResult<string> Name() =>
            this.Resolve(field => field.Name);

        public override IGraphQlObjectResult<__Type> Type() =>
            this.Resolve(field => field.FieldType).AsContract<GraphQlType>();
    }
}