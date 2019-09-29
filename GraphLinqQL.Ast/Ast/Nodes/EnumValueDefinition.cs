using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class EnumValueDefinition : NodeBase, IHasDirectives
    {
        public EnumValueDefinition(EnumValue enumValue, string? description, IEnumerable<Directive>? directives, LocationRange location) : base(location)
        {
            EnumValue = enumValue;
            Description = description;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.ScalarTypeDefinition;

        public IReadOnlyList<Directive> Directives { get; }
        public EnumValue EnumValue { get; }
        public string? Description { get; }
    }
}