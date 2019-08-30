using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQlResolver
{
    public static class Expressions
    {
        public static Expression<Func<TInput, TNewReturn>> ChangeReturnType<TInput, TOldReturn, TNewReturn>(this Expression<Func<TInput, TOldReturn>> expression)
        {
            return Expression.Lambda<Func<TInput, TNewReturn>>(Expression.Convert(expression.Body, typeof(TNewReturn)), expression.Parameters);
        }

        internal static Expression Replace(this Expression body, ParameterExpression from, ParameterExpression with)
        {
            return new ReplaceConstantExpression(from, with).Visit(body);
        }
    }
}
