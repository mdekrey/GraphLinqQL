using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public static class Expressions
    {
        internal static T Replace<T, U>(this T body, U from, U with)
            where T : Expression
            where U : Expression
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

        public static Expression MergeJoin(this Expression newRoot, ParameterExpression firstInput, ParameterExpression newParameter, IGraphQlJoin join, IDictionary<Expression, Expression> parameters, out ParameterExpression nextParameter)
        {
            var incomingType = newParameter.Type;
            var outgoingType = TypeSystem.GetElementType(join.Queryable.GetType());
            var switched = join.Queryable.Expression.Replace(join.Root.Expression, with: newRoot);

            if (switched is MethodCallExpression callExpression)
            {
                var typeArgs = callExpression.Method.GetGenericArguments();
                if (callExpression.Method.Name == nameof(Queryable.Join))
                {
                    var resultType = typeof(Tuple<,>).MakeGenericType(incomingType, outgoingType);
                    nextParameter = Expression.Parameter(resultType, "JoinTuple " + resultType.FullName);
                    var newTypeArgs = new[] { incomingType, typeArgs[1], typeArgs[2], resultType };
                    var newMethod = callExpression.Method.GetGenericMethodDefinition().MakeGenericMethod(newTypeArgs);
                    var keySelector = (LambdaExpression)((UnaryExpression)callExpression.Arguments[2]).Operand;
                    var newKeySelector = Expression.Lambda(keySelector.Body.Replace(keySelector.Parameters[0], parameters[firstInput]), newParameter);
                    var resultSelector = (LambdaExpression)((UnaryExpression)callExpression.Arguments[4]).Operand;
                    var newValueSelector = resultSelector.Replace(resultSelector.Parameters[0], parameters[firstInput]);
                    var tupleConstructor = Expression.New(resultType.GetConstructor(new[] { incomingType, outgoingType }), newParameter, newValueSelector.Body);
                    var newResultSelector = Expression.Lambda(tupleConstructor, newParameter, resultSelector.Parameters[1]);
                    var args = callExpression.Arguments.ToArray();
                    args[2] = Expression.Quote(newKeySelector);
                    args[4] = Expression.Quote(newResultSelector);
                    var result = Expression.Call(newMethod, args);
                    var newPath = (Expression)Expression.Property(nextParameter, resultType.GetProperty(nameof(Tuple<object, object>.Item1)));
                    foreach (var key in parameters.Keys.ToArray())
                    {
                        parameters[key] = parameters[key].Replace(newParameter, newPath);
                    }
                    parameters[join.Placeholder] = Expression.Property(nextParameter, resultType.GetProperty(nameof(Tuple<object, object>.Item2)));

                    return result;
                }
                else if (callExpression.Method.Name == nameof(Queryable.GroupJoin))
                {
                    // TODO - this needs serious work
                    var resultType = typeof(Tuple<,>).MakeGenericType(incomingType, outgoingType);
                    nextParameter = Expression.Parameter(resultType, "GroupJoinTuple " + resultType.FullName);
                    var newTypeArgs = new[] { incomingType, typeArgs[1], typeArgs[2], resultType };
                    var newMethod = callExpression.Method.GetGenericMethodDefinition().MakeGenericMethod(newTypeArgs);
                    var keySelector = (LambdaExpression)((UnaryExpression)callExpression.Arguments[2]).Operand;
                    var newKeySelector = Expression.Lambda(keySelector.Body.Replace(keySelector.Parameters[0], parameters[firstInput]), newParameter);
                    var resultSelector = (LambdaExpression)((UnaryExpression)callExpression.Arguments[4]).Operand;
                    var newValueSelector = resultSelector.Replace(resultSelector.Parameters[0], parameters[firstInput]);
                    var tupleConstructor = Expression.New(resultType.GetConstructor(new[] { incomingType, outgoingType }), newParameter, newValueSelector.Body);
                    var newResultSelector = Expression.Lambda(tupleConstructor, newParameter, resultSelector.Parameters[1]);
                    var args = callExpression.Arguments.ToArray();
                    args[2] = Expression.Quote(newKeySelector);
                    args[4] = Expression.Quote(newResultSelector);
                    var result = Expression.Call(newMethod, args);
                    var newPath = (Expression)Expression.Property(nextParameter, resultType.GetProperty(nameof(Tuple<object, object>.Item1)));
                    foreach (var key in parameters.Keys.ToArray())
                    {
                        parameters[key] = parameters[key].Replace(newParameter, newPath);
                    }
                    parameters[join.Placeholder] = Expression.Property(nextParameter, resultType.GetProperty(nameof(Tuple<object, object>.Item2)));

                    return result;
                }
            }
            throw new NotImplementedException();
        }
    }
}
