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

        public TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context)
        {
            return converter.VisitInt(this, context);
        }
    }
}
