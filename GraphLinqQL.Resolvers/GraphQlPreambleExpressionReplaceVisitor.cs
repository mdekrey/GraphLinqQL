using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    class PreambleReplacement
    {
        private class GraphQlPreambleExpressionReplaceVisitor : ExpressionVisitor
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
                    return body.Inline(argument.Box());
                }
                return base.VisitMethodCall(node);
            }
        }


        private readonly LambdaExpression body;

        public PreambleReplacement(LambdaExpression body)
        {
            this.body = body;
        }

        public LambdaExpression Replace(LambdaExpression expression)
        {
            var visitor = new GraphQlPreambleExpressionReplaceVisitor(body);

            var result = (LambdaExpression)visitor.Visit(expression);
            return visitor.Exchanged
                ? result
                : Expression.Lambda(body.Inline(expression.Body), expression.Parameters);
        }
    }

    public static class PreamblePlaceholders
    {
        private static readonly object internalObject = new object();
        public static readonly Expression BodyPlaceholderExpression = Expression.Constant(Expression.Lambda(Expression.Constant(internalObject)), typeof(LambdaExpression));
        public static readonly Expression<Func<object, object?>> BodyInvocationExpression =
            input => BodyInvocationPlaceholder(input);
        public static readonly MethodInfo BodyInvocationPlaceholderMethod = typeof(PreamblePlaceholders).GetMethod(nameof(BodyInvocationPlaceholder), BindingFlags.Static | BindingFlags.Public)!;


#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
        public static object? BodyInvocationPlaceholder(object? input) => null;
#pragma warning restore CA1801 // Remove unused parameter
    }
}