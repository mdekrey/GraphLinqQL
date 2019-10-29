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
    }
}