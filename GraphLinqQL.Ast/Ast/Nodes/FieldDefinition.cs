using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class FieldDefinition : NodeBase, IHasDirectives, IDefinitionNode
    {
        public FieldDefinition(
            string name,
            string? description,
            ITypeNode typeNode,
            IEnumerable<InputValueDefinition>? arguments,
            IEnumerable<Directive>? directives,
            LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            TypeNode = typeNode;
            Arguments = arguments?.ToArray() ?? EmptyArrayHelper.Empty<InputValueDefinition>();
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.FieldDefinition;

        public string Name { get; }
        public string? Description { get; }
        public ITypeNode TypeNode { get; }
        public IReadOnlyList<InputValueDefinition> Arguments { get; }
        public IReadOnlyList<Directive> Directives { get; }
    }
}