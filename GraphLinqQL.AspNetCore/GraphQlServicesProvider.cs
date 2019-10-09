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

        public IGraphQlResolvable GetResolverContract(Type contract)
        {
            return (IGraphQlResolvable)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, contract);
        }

        public IGraphQlTypeListing GetTypeListing()
        {
            return (IGraphQlTypeListing)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, GetTypeResolver().IntrospectionTypeListing);
        }

        public IGraphQlTypeResolver GetTypeResolver()
        {
            return (IGraphQlTypeResolver)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, options.TypeResolver);
        }

        public IGraphQlTypeInformation? TryGetTypeInformation(Type? type)
        {
            return type == null
                ? null
                : (IGraphQlTypeInformation)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    scope.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
