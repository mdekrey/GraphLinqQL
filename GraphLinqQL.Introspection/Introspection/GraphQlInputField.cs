using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlScalarResult<string?> defaultValue() =>
            Original.Resolve(field => field.DefaultValue);

        public override IGraphQlScalarResult<string?> description() =>
            Original.Resolve(field => field.Description);

        public override IGraphQlScalarResult<string> name() =>
            Original.Resolve(field => field.Name);

        public override IGraphQlObjectResult<__Type> type() =>
            Original.Resolve(field => field.FieldType).AsContract<GraphQlType>();
    }
}