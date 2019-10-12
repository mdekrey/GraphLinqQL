using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    class GraphQlPreambleExpressionReplaceVisitor : ExpressionVisitor
    {
        private readonly LambdaExpression body;

        public GraphQlPreambleExpressionReplaceVisitor(LambdaExpression body)
        {
            this.body = body;
        }

        public bool Exchanged { get; private set; } = false;

        public override Expression Visit(Expression node)
        {
            if (node == PreamblePlaceholders.BodyPlaceholderExpression)
            {
                Exchanged = true;
                return Expression.Quote(body);
            }
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == PreamblePlaceholders.BodyInvocationPlaceholderMethod)
            {
                Exchanged = true;
                var argument = node.Arguments[0] switch
                {
                    UnaryExpression { Operand: var actual, NodeType: ExpressionType.Convert } => actual,
                    var original => original
                };
                return body.Inline(argument);
            }
            return base.VisitMethodCall(node);
        }
    }

    public static class PreamblePlaceholders
    {
        public static readonly Expression BodyPlaceholderExpression = Expression.Constant(null, typeof(LambdaExpression));
        public static readonly Expression<Func<object, object?>> BodyInvocationExpression =
            input => BodyInvocationPlaceholder(input);
        public static readonly MethodInfo BodyInvocationPlaceholderMethod = typeof(PreamblePlaceholders).GetMethod(nameof(BodyInvocationPlaceholder), BindingFlags.Static | BindingFlags.Public)!;


#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
        public static object? BodyInvocationPlaceholder(object? input) => null;
#pragma warning restore CA1801 // Remove unused parameter
    }
}