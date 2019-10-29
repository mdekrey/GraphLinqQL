using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    public static class GraphQlContractExpression
    {
        public static readonly MethodInfo ContractPlaceholderMethod = typeof(GraphQlContractExpression).GetMethod(nameof(ContractPlaceholder), BindingFlags.Static | BindingFlags.NonPublic)!;

        public static Expression<Func<object, object?>> ResolveContractIndexed(int index)
        {
            var bodyParam = Expression.Parameter(typeof(object), "contractBody");
            return Expression.Lambda<Func<object, object?>>(Expression.Call(null, ContractPlaceholderMethod, bodyParam, Expression.Constant(index)), bodyParam);
        }


#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
#pragma warning disable IDE0060 // Remove unused parameter
        private static object? ContractPlaceholder(object input, int index) => null;
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Remove unused parameter

    }
}