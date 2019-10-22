using GraphLinqQL.Introspection.Interfaces.Introspection;
using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Text;
#pragma warning disable CA1801

namespace GraphLinqQL.Introspection
{
    public sealed class IntrospectionQuery<TQuery> : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        where TQuery : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
    {
        private readonly TQuery originalQuery;
        private readonly IGraphQlTypeListing typeListing;
        private readonly TypeListing introspectionTypeListing;

        public IntrospectionQuery(IGraphQlServiceProvider servicesProvider)
        {
            this.originalQuery = (TQuery)servicesProvider.GetResolverContract(typeof(TQuery));
            this.typeListing = servicesProvider.GetTypeListing();
            this.introspectionTypeListing = new Interfaces.Introspection.TypeListing();
        }

        public FieldContext FieldContext
        {
            get { return originalQuery.FieldContext; }
            set { originalQuery.FieldContext = value; }
        }

        IGraphQlResultFactory IGraphQlAccepts.Original { get => originalQuery.Original; set => originalQuery.Original = (IGraphQlResultFactory<GraphQlRoot>)value; }

        public string GraphQlTypeName => originalQuery.GraphQlTypeName;

        public bool IsType(string value) => originalQuery.IsType(value);

        internal IGraphQlObjectResult<Schema> schema() =>
            this.Original()!.Resolve(_ => typeListing).AsContract<Schema>();

        internal IGraphQlObjectResult<GraphQlType?> type(string name) =>
            this.Original()!.Resolve(_ => typeListing.Type(name) ?? introspectionTypeListing.Type(name)).Nullable(_ => _.AsContract<GraphQlType>());

        public IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__schema" => this.schema(),
                "__type" => this.type(name: parameters.GetParameter<string>("name")),
                _ => originalQuery.ResolveQuery(name, parameters)
            };
    }
}
