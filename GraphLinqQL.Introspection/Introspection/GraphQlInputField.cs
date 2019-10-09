using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlResult<string?> defaultValue(FieldContext fieldContext) =>
            Original.Resolve(field => field.DefaultValue);

        public override IGraphQlResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(field => field.Description);

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(field => field.Name);

        public override IGraphQlResult<__Type> type(FieldContext fieldContext) =>
            Original.Resolve(field => field.FieldType).AsContract<GraphQlType>();
    }
}