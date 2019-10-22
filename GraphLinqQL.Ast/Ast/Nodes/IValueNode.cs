using System;
using System.Collections.Generic;

namespace GraphLinqQL.Ast.Nodes
{
    public interface IValueNode : INode
    {
        TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context);
    }
}