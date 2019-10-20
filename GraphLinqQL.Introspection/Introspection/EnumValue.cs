namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlScalarResult<string?> DeprecationReason() =>
            this.Original().Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> Description() =>
            this.Original().Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> IsDeprecated() =>
            this.Original().Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> Name() =>
            this.Original().Resolve(v => v.Name);
    }
}