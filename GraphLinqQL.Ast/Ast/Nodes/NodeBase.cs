using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public abstract class NodeBase : INode
    {
        public NodeBase(LocationRange location)
        {
            this.Location = location;
        }

        public abstract NodeKind Kind { get; }

        public LocationRange Location { get; }
    }
}
