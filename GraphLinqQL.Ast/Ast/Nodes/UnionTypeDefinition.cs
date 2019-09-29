using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class UnionTypeDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public UnionTypeDefinition(string name, string? description, IEnumerable<Directive>? directives, IEnumerable<TypeName> unionMembers, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            UnionMembers = unionMembers.ToArray();
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.ObjectType;

        public string Name { get; }
        public string? Description { get; }
        public IEnumerable<TypeName> UnionMembers { get; }
        public IReadOnlyList<Directive> Directives { get; }
    }
}
