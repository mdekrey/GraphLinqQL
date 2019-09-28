using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class FragmentDefinition : NodeBase, IDefinitionNode, IHasDirectives
    {
        public FragmentDefinition(string name, TypeCondition typeCondition, IEnumerable<Directive>? directives, SelectionSet selectionSet, LocationRange location) : base(location)
        {
            Name = name;
            TypeCondition = typeCondition;
            SelectionSet = selectionSet;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.FragmentDefinition;

        public string Name { get; }
        public TypeCondition TypeCondition { get; }
        public SelectionSet SelectionSet { get; }
        public IReadOnlyList<Directive> Directives { get; }
    }
}
