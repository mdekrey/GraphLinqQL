using System;

namespace GraphLinqQL.Introspection
{
    public class GraphQlInputFieldInformation
    {
        public GraphQlInputFieldInformation(
            string name,
            Type type,
            string? description = null,
            string? defaultValue = null)
        {
            this.Description = description;
            this.Name = name;
            this.FieldType = type;
            this.DefaultValue = defaultValue;
        }

        public string? Description { get; }
        public string Name { get; }
        public Type FieldType { get; }
        public string? DefaultValue { get; }
    }
}