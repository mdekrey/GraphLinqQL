using System;

namespace GraphLinqQL
{
    public class GraphQlServiceProviderFactory : IGraphQlServiceProviderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public GraphQlServiceProviderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IGraphQlExecutionServiceProvider GetServiceProvider(GraphQlOptions options)
        {
            return new GraphQlServiceProvider(serviceProvider, options);
        }
    }
}
