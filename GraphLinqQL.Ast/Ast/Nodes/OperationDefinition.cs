using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{


    public class OperationDefinition : IDefinitionNode
    {
        public OperationDefinition(OperationType operationType, string? name, IEnumerable<VariableDefinition>? variables, SelectionSet selectionSet, Location location)
        {
            this.OperationType = operationType;
            this.Name = name;
            this.Variables = variables?.ToArray() ?? EmptyArrayHelper.Empty<VariableDefinition>();
            this.SelectionSet = selectionSet;
            this.Location = location;
        }

        public NodeKind Kind => NodeKind.OperationDefinition;

        public Location Location { get; }
        public OperationType OperationType { get; }
        public string? Name { get; }
        public IReadOnlyList<VariableDefinition> Variables { get; }
        public SelectionSet SelectionSet { get; }
    }
}
