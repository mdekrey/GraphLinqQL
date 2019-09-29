using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class ScalarTypeDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public ScalarTypeDefinition(string name, string? description, IEnumerable<Directive>? directives, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.ScalarTypeDefinition;

        public IReadOnlyList<Directive> Directives { get; }
        public string Name { get; }
        public string? Description { get; }
    }
}
