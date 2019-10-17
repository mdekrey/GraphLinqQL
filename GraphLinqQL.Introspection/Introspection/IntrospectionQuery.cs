﻿using GraphLinqQL.Introspection.Interfaces.Introspection;
using System;
using System.Collections.Generic;
using System.Text;
#pragma warning disable CA1801

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

        public FieldContext FieldContext
        {
            get { return originalQuery.FieldContext; }
            set { originalQuery.FieldContext = value; }
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

        public string GraphQlTypeName => originalQuery.GraphQlTypeName;

        public bool IsType(string value) => originalQuery.IsType(value);

        internal IGraphQlObjectResult<Schema> schema() =>
            original!.Resolve(_ => typeListing).AsContract<Schema>();

        internal IGraphQlObjectResult<GraphQlType?> type(string name) =>
            original!.Resolve(_ => typeListing.Type(name) ?? introspectionTypeListing.Type(name)).Nullable(_ => _.AsContract<GraphQlType>());

        public IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__schema" => this.schema(),
                "__type" => this.type(name: parameters.GetParameter<string>("name")),
                _ => originalQuery.ResolveQuery(name, parameters)
            };
    }
}
