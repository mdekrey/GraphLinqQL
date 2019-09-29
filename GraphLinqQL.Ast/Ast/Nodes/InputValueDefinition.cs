using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class InputValueDefinition : NodeBase, IHasDirectives
    {
        public InputValueDefinition(string name, string? description, ITypeNode typeNode, IValueNode? defaultValue, IEnumerable<Directive>? directives, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            TypeNode = typeNode;
            DefaultValue = defaultValue;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.InputValueDefinition;

        public string Name { get; }
        public string? Description { get; }
        public ITypeNode TypeNode { get; }
        public IValueNode? DefaultValue { get; }
        public IReadOnlyList<Directive> Directives { get; }
    }
}