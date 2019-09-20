using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public class GraphQlServicesProvider : IGraphQlServicesProvider
    {
        private readonly IServiceProvider serviceProvider;

        public GraphQlServicesProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IGraphQlParameterResolverFactory GetParameterResolverFactory()
        {
            return serviceProvider.GetRequiredService<IGraphQlParameterResolverFactory>();
        }

        public IGraphQlResolvable GetResolverContract(Type contract)
        {
            return (IGraphQlResolvable)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, contract);
        }
    }
}
