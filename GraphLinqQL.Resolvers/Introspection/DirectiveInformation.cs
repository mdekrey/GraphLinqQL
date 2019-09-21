using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace GraphLinqQL.Introspection
{
    public class DirectiveInformation
    {
        public DirectiveInformation(string name, DirectiveLocation[] locations, GraphQlInputFieldInformation[] args, string? description)
        {
            Name = name;
            Locations = locations.ToImmutableList();
            Description = description;
            Arguments = args.ToImmutableList();
        }

        public string? Description { get; }
        public string Name { get; }
        public IReadOnlyList<DirectiveLocation> Locations { get; }

        public IReadOnlyList<GraphQlInputFieldInformation> Arguments { get; }
    }
}
