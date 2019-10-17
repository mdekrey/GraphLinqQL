using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public class FinalizerContext
    {
        public FinalizerContext(CancellationToken cancellationToken, Func<object?, FinalizerContext, Task<object?>> unrollResults)
        {
            CancellationToken = cancellationToken;
            UnrollResults = unrollResults;
        }

        public CancellationToken CancellationToken { get; }
        public Func<object?, FinalizerContext, Task<object?>> UnrollResults { get; }
        public List<GraphQlError> Errors { get; } = new List<GraphQlError>();
    }
}