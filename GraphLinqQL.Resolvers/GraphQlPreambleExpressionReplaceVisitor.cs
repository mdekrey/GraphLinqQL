using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    class GraphQlPreambleExpressionReplaceVisitor : ExpressionVisitor
    {
        public static readonly Expression BodyPlaceholderExpression = Expression.Constant(null, typeof(LambdaExpression));
        public static readonly Expression<Func<object, object?>> BodyInvocationExpression = 
            input => BodyInvocationPlaceholder(input);
        public static readonly MethodInfo BodyInvocationPlaceholderMethod = typeof(GraphQlPreambleExpressionReplaceVisitor).GetMethod(nameof(BodyInvocationPlaceholder), BindingFlags.Static | BindingFlags.NonPublic)!;

        private readonly LambdaExpression body;

        public GraphQlPreambleExpressionReplaceVisitor(LambdaExpression body)
        {
            this.body = body;
        }

        public bool Exchanged { get; private set; } = false;

        public override Expression Visit(Expression node)
        {
            if (node == BodyPlaceholderExpression)
            {
                Exchanged = true;
                return Expression.Quote(body);
            }
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == BodyInvocationPlaceholderMethod)
            {
                Exchanged = true;
                return body.Inline(node.Arguments[0]);
            }
            return base.VisitMethodCall(node);
        }


#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
        private static object? BodyInvocationPlaceholder(object input) => null;
#pragma warning restore CA1801 // Remove unused parameter
    }
}