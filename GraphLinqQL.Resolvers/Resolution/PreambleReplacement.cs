using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
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
                    var argument = node.Arguments[0].Unbox();
                    return body.Inline(argument.Box()).Box();
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
                : Expression.Lambda(body.Inline(expression.Body.Unbox()).Box(), expression.Parameters);
        }


        private class GraphQlPreambleExpressionTypeVisitor : ExpressionVisitor
        {

            public Type? InnerType { get; private set; }

            public override Expression Visit(Expression node)
            {
                if (node == PreamblePlaceholders.BodyPlaceholderExpression)
                {
                    // TODO - is there a better way for this?
                    InnerType = typeof(object);
                }
                return base.Visit(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method == PreamblePlaceholders.BodyInvocationPlaceholderMethod)
                {
                    var argument = node.Arguments[0].Unbox();
                    InnerType = argument.Type;
                }
                return base.VisitMethodCall(node);
            }
        }

        public static Type GetExpectedReplacementType(LambdaExpression expression)
        {
            var visitor = new GraphQlPreambleExpressionTypeVisitor();
            visitor.Visit(expression);
            return visitor.InnerType ?? expression.ReturnType;
        }
    }
}