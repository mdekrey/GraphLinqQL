using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class ArrayValue : NodeBase, IValueNode
    {
        public ArrayValue(IEnumerable<IValueNode> values, LocationRange location) : base(location)
        {
            Values = values;
        }

        public override NodeKind Kind => NodeKind.ArrayValue;

        public IEnumerable<IValueNode> Values { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitArray(this, converterContext, expectedType, nullable);
        }
    }
}
