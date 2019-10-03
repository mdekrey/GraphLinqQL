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

        public TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context)
        {
            return converter.VisitArray(this, context);
        }
    }
}
