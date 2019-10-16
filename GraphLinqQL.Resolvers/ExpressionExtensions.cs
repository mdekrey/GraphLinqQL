using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    public static class ExpressionExtensions
    {
        private static readonly MethodInfo GenericQueryableSelect = typeof(System.Linq.Queryable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Queryable.Select))
                    // 2nd parameter should be Expression<Func<,>>
                    .Where(m => m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>))
                    .Single();
        private static readonly MethodInfo GenericEnumerableSelect = typeof(System.Linq.Enumerable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Enumerable.Select))
                    // 2nd parameter should be Func<,>
                    .Where(m => m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
                    .Single();

        internal static T Replace<T>(this T body, Expression from, Expression with)
            where T : Expression
        {
            return body.Replace(new Dictionary<Expression, Expression> { { from, with } });
        }

        internal static T Replace<T>(this T body, IDictionary<Expression, Expression> replacements)
            where T : Expression
        {
            return (T)new ReplaceConstantExpressions(replacements).Visit(body);
        }

        [ContractArgumentValidator]
        public static Expression<Func<TInput, object>> CastAndBoxSingleInput<TInput>(this LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            else if (expression.Parameters.Count != 1 || expression.Parameters[0].Type != typeof(TInput))
            {
                throw new InvalidOperationException($"Expected single input parameter of type {typeof(TInput).FullName}, got {string.Join(", ", expression.Parameters.Select(p => p.Type.FullName))}");
            }
            
            return Expression.Lambda<Func<TInput, object>>(expression.Body.Box(), expression.Parameters);
        }

        public static Expression Box(this Expression body)
        {
            return body.Type.IsValueType ? Expression.Convert(body, typeof(object)) : body;
        }

        public static Expression Unbox(this Expression expression)
        {
            return expression switch
            {
                UnaryExpression { Operand: var actual, NodeType: ExpressionType.Convert, Type: var t } when t == typeof(object) => actual,
                var original => original
            };
    }

        internal static MethodCallExpression CallSelect(this Expression list, LambdaExpression selector)
        {
            return list.IsQueryable()
                ? list.CallQueryableSelect(selector)
                : list.CallEnumerableSelect(selector);
        }

        internal static bool IsQueryable(this Expression list)
        {
            var elementType = TypeSystem.GetElementType(list.Type);
            return typeof(IQueryable<>).MakeGenericType(elementType).IsAssignableFrom(list.Type);
        }

        internal static MethodCallExpression CallQueryableSelect(this Expression list, LambdaExpression selector)
        {
            var queryableSelect = GenericQueryableSelect.MakeGenericMethod(new[] { TypeSystem.GetElementType(list.Type), selector.ReturnType });
            return Expression.Call(queryableSelect, list, Expression.Quote(selector));
        }

        internal static MethodCallExpression CallEnumerableSelect(this Expression list, LambdaExpression selector)
        {
            var enumerableSelect = GenericEnumerableSelect.MakeGenericMethod(new[] { TypeSystem.GetElementType(list.Type), selector.ReturnType });
            return Expression.Call(enumerableSelect, list, selector);
        }

        internal static Expression IfNotNull(this Expression maybeNull, Expression whenNotNull)
        {
            return Expression.Condition(Expression.ReferenceEqual(maybeNull, Expression.Constant(null)), Expression.Constant(null, whenNotNull.Type), whenNotNull);
        }
        
        internal static Expression Inline(this LambdaExpression newOperation, params Expression[] expressions)
        {
            var parameters = Enumerable.Zip(
                newOperation.Parameters,
                expressions, 
                (old, inlined) =>
                {
                    if (old.Type.IsValueType)
                    {
                        inlined = inlined switch
                        {
                            UnaryExpression { Type: var type, NodeType: ExpressionType.Convert, Operand: var expression } when type == typeof(object) => expression,
                            _ => inlined
                        };
                    }
                    return new { old, inlined };
                }
            ).ToDictionary(kvp => (Expression)kvp.old, kvp => kvp.inlined);
            if (parameters.Any(kvp => !kvp.Key.Type.IsAssignableFrom(kvp.Value.Type)))
            {
                throw new ArgumentException("Parameters did not match types");
            }
            return newOperation.Body.Replace(parameters);
        }
    }
}
