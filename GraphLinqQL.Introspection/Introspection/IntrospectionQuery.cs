using GraphLinqQL.Introspection.Interfaces.Introspection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Introspection
{
    public class IntrospectionQuery<TQuery> : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        where TQuery : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
    {
        private readonly TQuery originalQuery;
        private readonly IGraphQlTypeListing typeListing;
        private readonly TypeListing introspectionTypeListing;
        private IGraphQlResultFactory<GraphQlRoot>? original;

        public IntrospectionQuery(IGraphQlServiceProvider servicesProvider)
        {
            // FIXME: This shouldn't use a service provider, should it?
            this.originalQuery = (TQuery)servicesProvider.GetResolverContract(typeof(TQuery));
            this.typeListing = servicesProvider.GetTypeListing();
            this.introspectionTypeListing = new Interfaces.Introspection.TypeListing();
        }

        public IGraphQlResultFactory<GraphQlRoot> Original
        {
            get { return original!; }
            set
            {
                original = value;
                originalQuery.Original = value;
            }
        }
        public Type ModelType => originalQuery.ModelType;
        IGraphQlResultFactory IGraphQlAccepts.Original { set => Original = (IGraphQlResultFactory<GraphQlRoot>)value; }
        public bool IsType(string value) => originalQuery.IsType(value);

        internal IGraphQlResult<Schema> schema() =>
            original!.Resolve(_ => typeListing).As<Schema>();

        internal IGraphQlResult<GraphQlType?> type(string name) =>
            original!.Resolve(_ => typeListing.Type(name) ?? introspectionTypeListing.Type(name)).Nullable(_ => _.As<GraphQlType>());

        public IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__schema" => this.schema(),
                "__type" => this.type(name: parameters.GetParameter<string>("name")),
                _ => originalQuery.ResolveQuery(name, parameters)
            };
    }
}
