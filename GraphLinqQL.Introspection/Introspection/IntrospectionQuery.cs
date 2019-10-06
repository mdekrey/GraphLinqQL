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
        IGraphQlResultFactory IGraphQlAccepts.Original { set => Original = (IGraphQlResultFactory<GraphQlRoot>)value; }
        public bool IsType(string value) => originalQuery.IsType(value);

        internal IGraphQlResult<Schema> schema(FieldContext fieldContext) =>
            original!.Resolve(_ => typeListing).AsContract<Schema>();

        internal IGraphQlResult<GraphQlType?> type(FieldContext fieldContext, string name) =>
            original!.Resolve(_ => typeListing.Type(name) ?? introspectionTypeListing.Type(name)).Nullable(_ => _.AsContract<GraphQlType>());

        public IGraphQlResult ResolveQuery(string name, FieldContext fieldContext, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__schema" => this.schema(fieldContext),
                "__type" => this.type(fieldContext, name: parameters.GetParameter<string>("name")),
                _ => originalQuery.ResolveQuery(name, fieldContext, parameters)
            };
    }
}
