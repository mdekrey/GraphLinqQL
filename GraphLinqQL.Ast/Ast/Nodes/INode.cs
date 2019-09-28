using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public interface INode
    {
        NodeKind Kind { get; }

        Location Location { get; }
    }
}
