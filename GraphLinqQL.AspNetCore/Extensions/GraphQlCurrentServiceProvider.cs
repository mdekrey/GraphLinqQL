using GraphLinqQL;

namespace Microsoft.Extensions.DependencyInjection
{
    public class GraphQlCurrentServiceProvider
    {
        public IGraphQlExecutionServiceProvider? CurrentServiceProvider { get; set; }
    }
}