namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlResult<string?> deprecationReason(FieldContext fieldContext) =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(v => v.Description);

        public override IGraphQlResult<bool> isDeprecated(FieldContext fieldContext) =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(v => v.Name);
    }
}