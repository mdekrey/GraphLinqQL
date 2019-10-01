using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class DirectiveDefinition : NodeBase, IDefinitionNode
    {
        public DirectiveDefinition(string name, string? description, IEnumerable<string> directiveLocations, IEnumerable<InputValueDefinition>? arguments, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            DirectiveLocations = directiveLocations.ToArray();
            Arguments = arguments?.ToArray() ?? EmptyArrayHelper.Empty<InputValueDefinition>();
        }

        public override NodeKind Kind => NodeKind.DirectiveDefinition;

        public string Name { get; }
        public string? Description { get; }
        public IReadOnlyList<string> DirectiveLocations { get; }
        public IReadOnlyList<InputValueDefinition> Arguments { get; }
    }
}
