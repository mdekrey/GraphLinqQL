using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GraphLinqQL.Execution
{
    public static class GraphQlResultExtensions
    {
        public static ExecutionResult InvokeResult(this IGraphQlScalarResult resolved, object input)
        {
            if (resolved.Joins.Any())
            {
                throw new InvalidOperationException("Cannot join at the root level");
            }
            var func = Expression.Lambda<Func<object>>(resolved.UntypedResolver.Inline(Expression.Constant(input)));
            // TODO - get errors here
            return new ExecutionResult(false, func.Compile()(), EmptyArrayHelper.Empty<GraphQlError>());
        }

    }
}
