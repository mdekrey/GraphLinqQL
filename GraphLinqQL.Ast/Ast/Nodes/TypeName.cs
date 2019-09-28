using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class TypeName : NodeBase, ITypeNode
    {
        public TypeName(string name, LocationRange location) : base(location)
        {
            Name = name;
        }

        public override NodeKind Kind => NodeKind.TypeName;

        public string Name { get; }
    }
}
