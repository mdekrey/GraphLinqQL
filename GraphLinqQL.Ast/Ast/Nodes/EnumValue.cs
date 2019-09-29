using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class EnumValue : NodeBase, IValueNode
    {
        public EnumValue(string tokenValue, LocationRange location) : base(location)
        {
            TokenValue = tokenValue;
        }

        public override NodeKind Kind => NodeKind.EnumValue;

        public string TokenValue { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitEnum(this, converterContext, expectedType, nullable);
        }
    }
}
