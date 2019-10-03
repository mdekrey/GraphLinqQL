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

        public TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context)
        {
            return converter.VisitObject(this, context);
        }
    }
}
