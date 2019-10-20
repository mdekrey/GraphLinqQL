using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlScalarResult<string?> DefaultValue() =>
            this.Original().Resolve(field => field.DefaultValue);

        public override IGraphQlScalarResult<string?> Description() =>
            this.Original().Resolve(field => field.Description);

        public override IGraphQlScalarResult<string> Name() =>
            this.Original().Resolve(field => field.Name);

        public override IGraphQlObjectResult<__Type> Type() =>
            this.Original().Resolve(field => field.FieldType).AsContract<GraphQlType>();
    }
}