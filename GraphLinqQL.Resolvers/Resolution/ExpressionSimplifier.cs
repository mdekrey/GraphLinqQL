using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    internal class ExpressionSimplifier : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is MemberInitExpression memberInit && memberInit.NewExpression.Type.GetCustomAttribute<InlinableClassAttribute>() != null)
            {
                var binding = memberInit.Bindings.FirstOrDefault(binding => node.Member == binding.Member);
                return binding switch
                {
                    MemberAssignment { Expression: var expression } => base.Visit(expression),
                    MemberMemberBinding _ => base.VisitMember(node),
                    MemberListBinding _ => base.VisitMember(node),
                    _ => base.VisitMember(node),
                };
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var extracted = node.Constructor.GetParameters()
                .Select((param, index) => param.GetCustomAttribute<ExtractLambdaAttribute>() != null
                    ? ExtractLambda(node.Arguments[index] as UnaryExpression)
                    : node.Arguments[index])
                .ToArray();

            return base.VisitNew(Expression.New(node.Constructor, extracted));
        }

        private Expression ExtractLambda(UnaryExpression? expression)
        {
            var operand = expression?.Operand as LambdaExpression;
            if (operand == null)
            {
                return Expression.Constant(null, typeof(object));
            }
            var lambda = this.VisitAndConvert(operand, nameof(ExtractLambda));
            Expression<Func<LambdaExpression?>> temp = () => lambda;
            return temp.Body;
        }
    }
}