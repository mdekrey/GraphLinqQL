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
}