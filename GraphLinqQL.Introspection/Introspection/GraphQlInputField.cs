using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlScalarResult<string?> defaultValue(FieldContext fieldContext) =>
            Original.Resolve(field => field.DefaultValue);

        public override IGraphQlScalarResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(field => field.Description);

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(field => field.Name);

        public override IGraphQlObjectResult<__Type> type(FieldContext fieldContext) =>
            Original.Resolve(field => field.FieldType).AsContract<GraphQlType>();
    }
}