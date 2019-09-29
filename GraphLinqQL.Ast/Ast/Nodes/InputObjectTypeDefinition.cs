using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class InputObjectTypeDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public InputObjectTypeDefinition(string name, string? description, IEnumerable<Directive>? directives, IEnumerable<InputValueDefinition>? fields, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
            InputValues = fields?.ToArray() ?? EmptyArrayHelper.Empty<InputValueDefinition>();
        }

        public override NodeKind Kind => NodeKind.ObjectType;

        public string Name { get; }
        public string? Description { get; }
        public IReadOnlyList<Directive> Directives { get; }
        public IReadOnlyList<InputValueDefinition> InputValues { get; }
    }
}
