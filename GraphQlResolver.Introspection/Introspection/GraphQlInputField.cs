using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlResult<string?> defaultValue() =>
            Original.Resolve(field => field.DefaultValue);

        public override IGraphQlResult<string?> description() =>
            Original.Resolve(field => field.Description);

        public override IGraphQlResult<string> name() =>
            Original.Resolve(field => field.Name);

        public override IGraphQlResult<__Type> type() =>
            Original.Resolve(field => field.FieldType).Convertable().As<GraphQlType>();
    }
}