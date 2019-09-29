using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class EnumTypeDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public EnumTypeDefinition(string name, string? description, IEnumerable<EnumValueDefinition> enumValues, IEnumerable<Directive>? directives, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            EnumValues = enumValues.ToArray();
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.ScalarTypeDefinition;

        public IReadOnlyList<Directive> Directives { get; }
        public string Name { get; }
        public string? Description { get; }
        public IReadOnlyList<EnumValueDefinition> EnumValues { get; }
    }
}
