using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    public static class GraphQlContractExpression
    {
        public static readonly MethodInfo ContractPlaceholderMethod = typeof(GraphQlContractExpression).GetMethod(nameof(ContractPlaceholder), BindingFlags.Static | BindingFlags.NonPublic)!;

        public static Expression ResolveContract(Expression body, int index) =>
            Expression.Call(GraphQlContractExpression.ContractPlaceholderMethod, body, Expression.Constant(index));

#pragma warning disable CA1801 // Remove unused parameter - this parameter is used in Expression manipulation
#pragma warning disable IDE0060 // Remove unused parameter
        private static object? ContractPlaceholder(object input, int index) => null;
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Remove unused parameter

    }
}