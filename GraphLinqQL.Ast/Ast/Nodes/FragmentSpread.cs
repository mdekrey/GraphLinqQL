using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class FragmentSpread : NodeBase, IHasDirectives, ISelection
    {
        public FragmentSpread(string fragmentName, IEnumerable<Directive>? directives, LocationRange location) : base(location)
        {
            FragmentName = fragmentName;
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.FragmentSpread;

        public string FragmentName { get; }
        public IReadOnlyList<Directive> Directives { get; }
    }
}
