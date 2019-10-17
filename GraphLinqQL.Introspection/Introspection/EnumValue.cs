namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlScalarResult<string?> deprecationReason() =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> description() =>
            Original.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> isDeprecated() =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> name() =>
            Original.Resolve(v => v.Name);
    }
}