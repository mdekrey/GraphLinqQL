using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class StringValue : NodeBase, IValueNode
    {
        public StringValue(string quotedStringValue, Location location) : base(location)
        {
            QuotedStringValue = quotedStringValue;
        }

        public override NodeKind Kind => NodeKind.StringValue;

        public string QuotedStringValue { get; }
    }
}
