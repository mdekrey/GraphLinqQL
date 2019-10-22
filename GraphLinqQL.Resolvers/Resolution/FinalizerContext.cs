using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GraphLinqQL.Resolution
{
    public class FinalizerContext
    {
        public FinalizerContext(Func<object?, FinalizerContext, Task<object?>> unrollResults, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            UnrollResults = unrollResults;
        }

        public CancellationToken CancellationToken { get; }
        public Func<object?, FinalizerContext, Task<object?>> UnrollResults { get; }
        public List<GraphQlError> Errors { get; } = new List<GraphQlError>();
    }
}