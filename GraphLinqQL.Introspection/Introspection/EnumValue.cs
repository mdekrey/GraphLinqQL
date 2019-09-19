namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlResult<string?> deprecationReason() =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlResult<string?> description() =>
            Original.Resolve(v => v.Description);

        public override IGraphQlResult<bool> isDeprecated() =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlResult<string> name() =>
            Original.Resolve(v => v.Name);
    }
}