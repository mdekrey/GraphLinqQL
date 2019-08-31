using System;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public static class Expressions
    {
        internal static Expression Replace(this Expression body, ParameterExpression from, ParameterExpression with)
        {
            return new ReplaceConstantExpression(from, with).Visit(body);
        }


        public static Expression<Func<TInput, object>> CastAndBoxSingleInput<TInput>(this LambdaExpression expression)
        {
            if (expression.Parameters.Count != 1 || expression.Parameters[0].Type != typeof(TInput))
            {
                throw new InvalidOperationException($"Expected single input parameter of type {typeof(TInput).FullName}, got {string.Join(", ", expression.Parameters.Select(p => p.Type.FullName))}");
            }
            return Expression.Lambda<Func<TInput, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters);
        }

    }
}
