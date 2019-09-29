using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class SchemaDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public SchemaDefinition(string? description, IEnumerable<Directive>? directives, IEnumerable<OperationTypeDefinition> operationTypes, LocationRange location) : base(location)
        {
            Description = description;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
            OperationTypes = operationTypes.ToArray();
        }

        public override NodeKind Kind => NodeKind.SchemaDefinition;

        public string? Description { get; }
        public IReadOnlyList<Directive> Directives { get; }
        public IReadOnlyList<OperationTypeDefinition> OperationTypes { get; }
    }
}
