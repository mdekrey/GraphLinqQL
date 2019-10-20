namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlScalarResult<string?> DeprecationReason() =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> Description() =>
            Original.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> IsDeprecated() =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> Name() =>
            Original.Resolve(v => v.Name);
    }
}