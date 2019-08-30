using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    public static class Expressions
    {
        public static Expression<Func<TNewInput, TReturn>> ChangeInputType<TOldInput, TNewInput, TReturn>(this Expression<Func<TOldInput, TReturn>> expression)
        {
            if (typeof(TNewInput) != typeof(TOldInput))
            {
                throw new InvalidOperationException($"Expected input type of {typeof(TNewInput).FullName}, got {typeof(TOldInput).FullName}");
            }
            // Yeah, this looks unchecked, but the if statement above fixes it
            return (Expression<Func<TNewInput, TReturn>>)(Expression)expression;
        }

        public static Expression<Func<TInput, object>> BoxReturnValue<TInput, TOldReturn>(this Expression<Func<TInput, TOldReturn>> expression)
        {
            return Expression.Lambda<Func<TInput, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters);
        }

        internal static Expression Replace(this Expression body, ParameterExpression from, ParameterExpression with)
        {
            return new ReplaceConstantExpression(from, with).Visit(body);
        }
    }
}
