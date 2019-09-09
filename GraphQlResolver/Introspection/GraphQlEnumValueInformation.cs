namespace GraphQlResolver.Introspection
{
    public class GraphQlEnumValueInformation
    {
        public GraphQlEnumValueInformation(
            string name,
            string? description = null,
            bool isDeprecated = false,
            string? deprecationReason = null)
        {
            this.DeprecationReason = deprecationReason;
            this.IsDeprecated = isDeprecated;
            this.Description = description;
            this.Name = name;
        }

        public string? DeprecationReason { get; }
        public bool IsDeprecated { get; }
        public string? Description { get; }
        public string Name { get; }
    }
}