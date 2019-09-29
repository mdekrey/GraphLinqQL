using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class FloatValue : NodeBase, IValueNode
    {
        public FloatValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue;
        }

        public override NodeKind Kind => NodeKind.FloatValue;

        public string TokenValue { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitFloat(this, converterContext, expectedType, nullable);
        }
    }
}
