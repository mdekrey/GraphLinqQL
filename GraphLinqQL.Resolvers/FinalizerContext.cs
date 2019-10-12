using System.Threading;

namespace GraphLinqQL
{
    public class FinalizerContext
    {
        public FinalizerContext(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        // TODO - what else should be here? Error lists?
        public CancellationToken CancellationToken { get; }

    }
}