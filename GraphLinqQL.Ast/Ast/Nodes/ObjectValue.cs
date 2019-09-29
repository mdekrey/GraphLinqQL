using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraphLinqQL.Ast.Nodes
{
    public class ObjectValue : NodeBase, IValueNode
    {
        public ObjectValue(IDictionary<string, IValueNode> fields, LocationRange location) : base(location)
        {
            Fields = new ReadOnlyDictionary<string, IValueNode>(new Dictionary<string, IValueNode>(fields));
        }

        public override NodeKind Kind => NodeKind.ArrayValue;

        public IReadOnlyDictionary<string, IValueNode> Fields { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitObject(this, converterContext, expectedType, nullable);
        }
    }
}
