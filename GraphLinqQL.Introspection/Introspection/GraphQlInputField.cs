using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlScalarResult<string?> DefaultValue() =>
            Original.Resolve(field => field.DefaultValue);

        public override IGraphQlScalarResult<string?> Description() =>
            Original.Resolve(field => field.Description);

        public override IGraphQlScalarResult<string> Name() =>
            Original.Resolve(field => field.Name);

        public override IGraphQlObjectResult<__Type> Type() =>
            Original.Resolve(field => field.FieldType).AsContract<GraphQlType>();
    }
}