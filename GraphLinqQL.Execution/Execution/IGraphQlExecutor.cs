using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;

namespace GraphLinqQL.Execution
{
    public interface IGraphQlExecutor : IDisposable
    {
        ExecutionResult Execute(string query, IDictionary<string, IGraphQlParameterInfo>? arguments = null);
    }
}