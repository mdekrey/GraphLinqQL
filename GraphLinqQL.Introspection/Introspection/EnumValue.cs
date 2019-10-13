namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlScalarResult<string?> deprecationReason(FieldContext fieldContext) =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> isDeprecated(FieldContext fieldContext) =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(v => v.Name);
    }
}