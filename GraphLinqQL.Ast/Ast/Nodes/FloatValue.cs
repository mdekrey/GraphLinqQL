﻿namespace GraphLinqQL.Ast.Nodes
{
    public class FloatValue : NodeBase, IValueNode
    {
        public FloatValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue;
        }

        public override NodeKind Kind => NodeKind.FloatValue;

        public string TokenValue { get; }
    }
}
