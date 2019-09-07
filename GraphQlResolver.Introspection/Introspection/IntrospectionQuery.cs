using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver.Introspection
{
    public class IntrospectionQuery<TQuery, TTypeListing> : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        where TQuery : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        where TTypeListing : IGraphQlTypeListing
    {
        private readonly TQuery originalQuery;
        private readonly IGraphQlTypeListing typeListing;
        private IGraphQlResultFactory<GraphQlRoot>? original;

        public IntrospectionQuery(IServiceProvider serviceProvider)
        {
            this.originalQuery = ActivatorUtilities.GetServiceOrCreateInstance<TQuery>(serviceProvider);
            this.typeListing = ActivatorUtilities.GetServiceOrCreateInstance<TTypeListing>(serviceProvider);
        }

        public IGraphQlResultFactory<GraphQlRoot> Original { set => original = originalQuery.Original = value; }
        public Type ModelType => originalQuery.ModelType;
        IGraphQlResultFactory IGraphQlAccepts.Original { set => Original = (IGraphQlResultFactory<GraphQlRoot>)value; }
        public bool IsType(string value) => originalQuery.IsType(value);

        internal IGraphQlResult<Schema> schema() =>
            original!.Resolve(_ => typeListing).Convertable().As<Schema>();

        internal IGraphQlResult<GraphQlType> type(string name) =>
            original!.Resolve(_ => typeListing.Type(name)).Convertable().As<GraphQlType>();

        public IGraphQlResult ResolveQuery(string name, IDictionary<string, object?> parameters) =>
            name switch
            {
                "__schema" => this.schema(),
                "__type" => this.type(name: (string)parameters["name"]!),
                _ => originalQuery.ResolveQuery(name, parameters)
            };
    }
}
