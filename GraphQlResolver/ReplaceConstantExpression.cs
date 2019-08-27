using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    class ReplaceConstantExpression : ExpressionVisitor
    {
        private readonly Expression target;
        private readonly Expression replacement;

        public ReplaceConstantExpression(Expression target, Expression replacement)
        {
            this.target = target;
            this.replacement = replacement;
        }

        public override Expression Visit(Expression node)
        {
            if (node == target)
            {
                return replacement;
            }
            return base.Visit(node);
        }
    }
}
