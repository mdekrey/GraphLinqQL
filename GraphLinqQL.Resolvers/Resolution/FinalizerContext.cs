using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GraphLinqQL.Resolution
{
    public class FinalizerContext
    {
        public FinalizerContext(Func<object?, FinalizerContext, Task<object?>> unrollResults, Microsoft.Extensions.Logging.ILogger logger, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            UnrollResults = unrollResults;
            Logger = logger;
        }

        public CancellationToken CancellationToken { get; }
        public Func<object?, FinalizerContext, Task<object?>> UnrollResults { get; }
        public Microsoft.Extensions.Logging.ILogger Logger { get; }
        public List<GraphQlError> Errors { get; } = new List<GraphQlError>();
    }
}