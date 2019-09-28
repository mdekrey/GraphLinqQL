using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class SelectionSet : NodeBase
    {
        public SelectionSet(IEnumerable<ISelection> selections, LocationRange location)
            : base(location)
        {
            Selections = selections.ToArray();
        }

        public override NodeKind Kind => NodeKind.SelectionSet;

        public IReadOnlyList<ISelection> Selections { get; }
    }
}