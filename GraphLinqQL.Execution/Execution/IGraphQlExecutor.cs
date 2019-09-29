using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;

namespace GraphLinqQL.Execution
{
    public interface IGraphQlExecutor : IDisposable
    {
        object Execute(string query, IDictionary<string, IGraphQlParameterInfo>? arguments = null);
    }
}