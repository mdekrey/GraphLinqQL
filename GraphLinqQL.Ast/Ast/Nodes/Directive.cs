using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class Directive : NodeBase
    {
        public Directive(string name, IEnumerable<Argument>? arguments, LocationRange location) : base(location)
        {
            Name = name;
            Arguments = arguments?.ToArray() ?? EmptyArrayHelper.Empty<Argument>();
        }

        public override NodeKind Kind => NodeKind.Directive;

        public string Name { get; }
        public IReadOnlyList<Argument> Arguments { get; }
    }
}