using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class IntValue : NodeBase, IValueNode
    {
        public IntValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue;
        }

        public override NodeKind Kind => NodeKind.IntValue;

        public string TokenValue { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitInt(this, converterContext, expectedType, nullable);
        }
    }
}
