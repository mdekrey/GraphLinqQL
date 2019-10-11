using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    class GraphQlContractExpressionReplaceVisitor : ExpressionVisitor
    {
        public static readonly MethodInfo ContractPlaceholderMethod = typeof(GraphQlContractExpressionReplaceVisitor).GetMethod(nameof(ContractPlaceholder), BindingFlags.Static | BindingFlags.NonPublic)!;

#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
        private static object? ContractPlaceholder(object input) => null;
#pragma warning restore CA1801 // Remove unused parameter

        public LambdaExpression? NewOperation { get; set; }

        public Type? ModelType { get; set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == ContractPlaceholderMethod)
            {
                ModelType = node.Arguments[0].Type;
                if (NewOperation != null)
                {
                    return Visit(NewOperation.Inline(node.Arguments[0]));
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}