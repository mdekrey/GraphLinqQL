using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace GraphLinqQL.Execution
{
    internal class GraphQlExecutorFactory : IGraphQlExecutorFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IOptionsMonitor<GraphQlOptions> options;

        public GraphQlExecutorFactory(IServiceProvider serviceProvider, IOptionsMonitor<GraphQlOptions> options)
        {
            this.serviceProvider = serviceProvider;
            this.options = options;
        }

        public IGraphQlExecutor Create() =>
            Create(options.Get(Options.DefaultName));

        public IGraphQlExecutor Create(string name) =>
            Create(options.Get(name));

        private IGraphQlExecutor Create(GraphQlOptions options)
        {
            return new GraphQlExecutor(serviceProvider.GetRequiredService<IGraphQlServicesProvider>(), new GraphQlExecutionOptions()
            {
                Query = options.Query,
                Mutation = options.Mutation,
                Subscription = options.Subscription,
                Directives = options.Directives.Select(directiveType => ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, directiveType)).OfType<IGraphQlDirective>().ToImmutableList(),
                TypeResolver = (ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, options.TypeResolver) as IGraphQlTypeResolver)
                    ?? throw new InvalidOperationException($"{nameof(options.TypeResolver)} was not of type {nameof(IGraphQlTypeResolver)}")
            });
        }

        private class GraphQlExecutionOptions : IGraphQlExecutionOptions
        {
            public Type? Query { get; set; }

            public Type? Mutation { get; set; }

            public Type? Subscription { get; set; }

#nullable disable
            public IReadOnlyList<IGraphQlDirective> Directives { get; set; }

            public IGraphQlTypeResolver TypeResolver { get; set; }
#nullable restore
        }
    }
}