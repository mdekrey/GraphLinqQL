using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    class GraphQlContractExpressionReplaceVisitor : ExpressionVisitor
    {
        public IReadOnlyList<LambdaExpression>? NewOperations { get; set; }

        public Type? ModelType { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == GraphQlContractExpression.ContractPlaceholderMethod)
            {
                var arg0 = Visit(node.Arguments[0]);
                ModelType = arg0.Type;
                if (NewOperations != null)
                {
                    var arg = node.Arguments[1];
                    return NewOperations[(int)((ConstantExpression)arg).Value].Inline(arg0);
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}