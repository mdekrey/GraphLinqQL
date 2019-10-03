using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class NullValue : NodeBase, IValueNode
    {
        public NullValue(LocationRange location) : base(location)
        {
        }

        public override NodeKind Kind => NodeKind.NullValue;

        public TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context)
        {
            return converter.VisitNull(this, context);
        }
    }
}
