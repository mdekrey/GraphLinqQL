using System;
using System.Collections.Generic;

namespace GraphQlResolver.Execution
{
    public delegate IDictionary<string, object?> GraphQlArgumentsSupplier(IDictionary<string, Type> argumentTypes);

    public interface IGraphQlExecutor
    {
        object Execute(string query, GraphQlArgumentsSupplier argumentsSupplier);
    }
}