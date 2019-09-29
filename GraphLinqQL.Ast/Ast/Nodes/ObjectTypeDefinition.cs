using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class ObjectTypeDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public ObjectTypeDefinition(string name, string? description, IEnumerable<TypeName>? interfaces, IEnumerable<Directive>? directives, IEnumerable<FieldDefinition>? fields, LocationRange location) : base(location)
        {
            Name = name;
            Description = description;
            Interfaces = interfaces?.ToArray() ?? EmptyArrayHelper.Empty<TypeName>();
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
            Fields = fields?.ToArray() ?? EmptyArrayHelper.Empty<FieldDefinition>();
        }

        public override NodeKind Kind => NodeKind.ObjectType;

        public string Name { get; }
        public string? Description { get; }
        public IReadOnlyList<TypeName> Interfaces { get; }
        public IReadOnlyList<Directive> Directives { get; }
        public IReadOnlyList<FieldDefinition> Fields { get; }
    }
}
