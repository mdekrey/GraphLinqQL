namespace GraphLinqQL.Introspection
{
    internal class EnumValue : Interfaces.__EnumValue.GraphQlContract<GraphQlEnumValueInformation>
    {
        public override IGraphQlScalarResult<string?> DeprecationReason() =>
            this.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> Description() =>
            this.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> IsDeprecated() =>
            this.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> Name() =>
            this.Resolve(v => v.Name);
    }
}