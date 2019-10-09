using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphLinqQL
{
    public class FieldContext
    {
        public static readonly FieldContext Empty = new FieldContext(null, ImmutableList<QueryLocation>.Empty);

        public FieldContext(string? name, IReadOnlyList<QueryLocation> locations)
        {
            Name = name;
            Locations = locations;
        }

        public string? Name { get; }
        public IReadOnlyList<QueryLocation> Locations { get; }
    }
}
