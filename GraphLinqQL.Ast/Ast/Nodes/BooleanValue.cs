using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class BooleanValue : NodeBase, IValueNode
    {
        public BooleanValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue == "true";
        }

        public override NodeKind Kind => NodeKind.BooleanValue;

        public bool TokenValue { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitBoolean(this, converterContext, expectedType, nullable);
        }
    }
}
