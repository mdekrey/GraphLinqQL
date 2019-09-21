using GraphLinqQL.Introspection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public class GraphQlServiceProvider : IGraphQlExecutionServiceProvider
    {
        private readonly IServiceScope scope;
        private readonly IServiceProvider serviceProvider;
        private readonly GraphQlOptions options;

        public GraphQlServiceProvider(IServiceProvider serviceProvider, GraphQlOptions options)
        {
            scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<GraphQlCurrentServiceProvider>().CurrentServiceProvider = this;
            
            this.serviceProvider = scope.ServiceProvider;
            this.options = options;
        }

        public IGraphQlDirective GetDirective(Type directive)
        {
            return (IGraphQlDirective)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, directive);
        }

        public IGraphQlParameterResolverFactory GetParameterResolverFactory()
        {
            return serviceProvider.GetRequiredService<IGraphQlParameterResolverFactory>();
        }

        public IGraphQlResolvable GetResolverContract(Type contract)
        {
            return (IGraphQlResolvable)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, contract);
        }

        public IGraphQlTypeListing GetTypeListing()
        {
            return (IGraphQlTypeListing)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, options.TypeListing);
        }

        public IGraphQlTypeResolver? GetTypeResolver()
        {
            return (IGraphQlTypeResolver)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, options.TypeResolver);
        }

        public IGraphQlTypeInformation? TryGetTypeInformation(Type? type)
        {
            return type == null
                ? null
                : (IGraphQlTypeInformation)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
        }

        void IDisposable.Dispose()
        {
            scope.Dispose();
        }

    }
}
