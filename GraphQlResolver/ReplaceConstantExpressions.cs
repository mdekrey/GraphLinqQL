using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    class ReplaceConstantExpressions : ExpressionVisitor
    {
        private IDictionary<Expression, Expression> replacements;

        public ReplaceConstantExpressions(IDictionary<Expression, Expression> replacements)
        {
            this.replacements = replacements;
        }

        public override Expression Visit(Expression node)
        {
            if (node != null && replacements.ContainsKey(node))
            {
                return replacements[node];
            }
            return base.Visit(node);
        }
    }
}
