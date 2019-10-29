using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    public static class PreamblePlaceholders
    {
        //private static readonly object internalObject = new object();
        public static readonly Expression BodyPlaceholderExpression = Expression.Parameter(typeof(LambdaExpression), nameof(BodyPlaceholderExpression)); //Expression.Constant(Expression.Lambda(Expression.Constant(internalObject)), typeof(LambdaExpression));
        public static readonly Expression<Func<object, object?>> BodyInvocationExpression =
            input => BodyInvocationPlaceholder(input);
        public static readonly MethodInfo BodyInvocationPlaceholderMethod = typeof(PreamblePlaceholders).GetMethod(nameof(BodyInvocationPlaceholder), BindingFlags.Static | BindingFlags.Public)!;


#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
#pragma warning disable IDE0060
        public static object? BodyInvocationPlaceholder(object? input) => null;
#pragma warning restore CA1801 // Remove unused parameter
    }
}