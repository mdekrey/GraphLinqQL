using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class ListType : NodeBase, ITypeNode
    {
        public ListType(ITypeNode elementType, Location location) : base(location)
        {
            ElementType = elementType;
        }

        public override NodeKind Kind => NodeKind.ListType;

        public ITypeNode ElementType { get; }
    }
}
