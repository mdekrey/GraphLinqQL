using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using GraphLinqQL.Ast;

namespace GraphLinqQL.Execution
{
    internal class GraphQlExecutorFactory : IGraphQlExecutorFactory
    {
        private readonly IGraphQlServiceProviderFactory serviceProviderFactory;
        private readonly IAbstractSyntaxTreeGenerator astGenerator;
        private readonly IOptionsMonitor<GraphQlOptions> options;

        public GraphQlExecutorFactory(IGraphQlServiceProviderFactory serviceProviderFactory, IAbstractSyntaxTreeGenerator astGenerator, IOptionsMonitor<GraphQlOptions> options)
        {
            this.serviceProviderFactory = serviceProviderFactory;
            this.astGenerator = astGenerator;
            this.options = options;
        }

        public IGraphQlExecutor Create() =>
            Create(options.Get(Options.DefaultName));

        public IGraphQlExecutor Create(string name) =>
            Create(options.Get(name));

        private IGraphQlExecutor Create(GraphQlOptions options)
        {
            var serviceProvider = serviceProviderFactory.GetServiceProvider(options);

            return new GraphQlExecutor(serviceProvider, astGenerator, new GraphQlExecutionOptions()
            {
                Query = options.Query,
                Mutation = options.Mutation,
                Subscription = options.Subscription,
                Directives = options.Directives.Select(directiveType => serviceProvider.GetDirective(directiveType)).ToImmutableList(),
                TypeResolver = serviceProvider.GetTypeResolver()
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