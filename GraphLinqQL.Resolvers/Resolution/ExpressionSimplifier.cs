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

        //protected override Expression VisitMethodCall(MethodCallExpression node)
        //{
        //    var extracted = node.Method.GetParameters()
        //        .Select((param, index) => param.GetCustomAttribute<ExtractLambdaAttribute>() != null
        //            ? ExtractLambda(node.Arguments[index] as UnaryExpression)
        //            : node.Arguments[index])
        //        .ToArray();

        //    return base.VisitMethodCall(Expression.Call(node.Object, node.Method, extracted));
        //}

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
            var lambda = expression?.Operand as LambdaExpression;
            if (lambda == null)
            {
                throw new InvalidOperationException();
            }
            Expression<Func<LambdaExpression?>> temp = () => lambda;
            return temp.Body;
        }
    }
}