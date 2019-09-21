using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL.Introspection
{
    public class GraphQlFieldInformation
    {

        public GraphQlFieldInformation(
            Type type,
            string name,
            GraphQlInputFieldInformation[] args,
            string? description = null,
            bool isDeprecated = false,
            string? deprecationReason = null)
        {
            this.FieldType = type;
            this.DeprecationReason = deprecationReason;
            this.Arguments = args.ToImmutableList();
            this.IsDeprecated = isDeprecated;
            this.Description = description;
            this.Name = name;
        }

        public Type FieldType { get; }
        public string? DeprecationReason { get; }
        public bool IsDeprecated { get; }
        public string? Description { get; }
        public string Name { get; }

        public IReadOnlyList<GraphQlInputFieldInformation> Arguments { get; }
    }
}