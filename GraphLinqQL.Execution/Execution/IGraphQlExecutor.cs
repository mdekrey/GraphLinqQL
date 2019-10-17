using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GraphLinqQL.Execution
{
    public interface IGraphQlExecutor : IDisposable
    {
        IGraphQlServiceProvider ServiceProvider { get; }
        Task<ExecutionResult> ExecuteAsync(string query, string? operationName = null, IDictionary<string, IGraphQlParameterInfo>? arguments = null, CancellationToken cancellationToken = default);
    }
}