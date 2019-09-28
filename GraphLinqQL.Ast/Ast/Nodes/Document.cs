using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast.Nodes
{
    public class Document : NodeBase
    {
        public Document(IEnumerable<IDefinitionNode> children, LocationRange location)
            : base(location)
        {
            this.Children = children.ToArray();
        }

        public override NodeKind Kind => NodeKind.Document;

        public IReadOnlyList<IDefinitionNode> Children { get; }
    }
}