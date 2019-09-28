﻿namespace GraphLinqQL.Ast.Nodes
{
    public class BooleanValue : NodeBase, IValueNode
    {
        public BooleanValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue == "true";
        }

        public override NodeKind Kind => NodeKind.BooleanValue;

        public bool TokenValue { get; }
    }
}
