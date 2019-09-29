using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class InlineFragment : NodeBase, IHasDirectives, ISelection
    {
        public InlineFragment(TypeCondition? typeCondition, IEnumerable<Directive>? directives, SelectionSet selectionSet, LocationRange location) : base(location)
        {
            TypeCondition = typeCondition;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
            SelectionSet = selectionSet;
        }

        public override NodeKind Kind => NodeKind.InlineFragment;

        public TypeCondition? TypeCondition { get; }
        public IReadOnlyList<Directive> Directives { get; }
        public SelectionSet SelectionSet { get; }
    }
}
