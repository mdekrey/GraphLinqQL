using System;
using System.Collections.Generic;

namespace GraphLinqQL.Execution
{
    public interface IGraphQlExecutor : IDisposable
    {
        object Execute(string query, IDictionary<string, string>? arguments = null);
    }
}