using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class NullValue : NodeBase, IValueNode
    {
        public NullValue(LocationRange location) : base(location)
        {
        }

        public override NodeKind Kind => NodeKind.NullValue;

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitNull(this, converterContext, expectedType, nullable);
        }
    }
}
