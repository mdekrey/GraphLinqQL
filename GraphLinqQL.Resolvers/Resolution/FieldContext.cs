using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphLinqQL.Resolution
{
    public class FieldContext
    {
        public static readonly FieldContext Empty = new FieldContext(null, null, ImmutableList<QueryLocation>.Empty);

        public FieldContext(string? typeName, string? name, IReadOnlyList<QueryLocation> locations)
        {
            TypeName = typeName;
            Name = name;
            Locations = locations;
        }

        public string? TypeName { get; }
        public string? Name { get; }
        public IReadOnlyList<QueryLocation> Locations { get; }
    }
}
