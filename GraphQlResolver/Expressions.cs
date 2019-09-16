using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    public static class Expressions
    {
        private static readonly MethodInfo GenericQueryableSelect = typeof(System.Linq.Queryable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Queryable.Select))
                    // 2nd parameter should be Expression<Func<,>>
                    .Where(m => m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>))
                    .Single();

        internal static T Replace<T>(this T body, Expression from, Expression with)
            where T : Expression
        {
            return (T)new ReplaceConstantExpression(from, with).Visit(body);
        }


        public static Expression<Func<TInput, object>> CastAndBoxSingleInput<TInput>(this LambdaExpression expression)
        {
            if (expression.Parameters.Count != 1 || expression.Parameters[0].Type != typeof(TInput))
            {
                throw new InvalidOperationException($"Expected single input parameter of type {typeof(TInput).FullName}, got {string.Join(", ", expression.Parameters.Select(p => p.Type.FullName))}");
            }
            return Expression.Lambda<Func<TInput, object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters);
        }

        public static Expression MergeJoin(this Expression newRoot, ParameterExpression joinPlaceholderParameter, IGraphQlJoin join, IDictionary<Expression, Expression> parameters)
        {
            System.Diagnostics.Debug.Assert(newRoot.Type == typeof(IQueryable<>).MakeGenericType(joinPlaceholderParameter.Type));
            System.Diagnostics.Debug.Assert(joinPlaceholderParameter.Type == typeof(JoinPlaceholder<>).MakeGenericType(join.Conversion.Parameters[0].Type));

            var joinConstant = Expression.Constant(join);
            var selectRemaining = join.Conversion.Body.Replace(join.Conversion.Parameters[0], with: Expression.Property(joinPlaceholderParameter, nameof(JoinPlaceholder<object>.Original)));

            var addMethod = joinPlaceholderParameter.Type.GetMethod(nameof(JoinPlaceholder<object>.Add)).MakeGenericMethod(join.Conversion.ReturnType);
            var getMethod = joinPlaceholderParameter.Type.GetMethod(nameof(JoinPlaceholder<object>.Get)).MakeGenericMethod(join.Conversion.ReturnType);
            var selectMethod = GenericQueryableSelect.MakeGenericMethod(joinPlaceholderParameter.Type, joinPlaceholderParameter.Type);
            var selectBody = Expression.Call(joinPlaceholderParameter, addMethod, joinConstant, selectRemaining);
            var result = Expression.Call(selectMethod, newRoot, Expression.Quote(Expression.Lambda(selectBody, joinPlaceholderParameter)));

            parameters[join.Placeholder] = Expression.Call(joinPlaceholderParameter, getMethod, joinConstant);
            return result;
        }

        internal static MethodCallExpression CallQueryableSelect(Expression list, LambdaExpression selector)
        {
            var queryableSelect = GenericQueryableSelect.MakeGenericMethod(new[] { TypeSystem.GetElementType(list.Type), selector.ReturnType });
            return Expression.Call(queryableSelect, list, Expression.Quote(selector));
        }
    }
}
