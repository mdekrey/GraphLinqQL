using System;
using System.Collections.Generic;
using System.Linq;
using GraphQlResolver.Introspection.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver.Introspection
{
    class Schema : Interfaces.__Schema.GraphQlContract<IGraphQlTypeListing>
    {
        private static readonly Type[] introspectionTypes = 
                new []
                {
                    typeof(Introspection.Interfaces.Introspection.__Schema),
                    typeof(Introspection.Interfaces.Introspection.__Type),
                    typeof(Introspection.Interfaces.Introspection.__TypeKind),
                    typeof(Introspection.Interfaces.Introspection.__Field),
                    typeof(Introspection.Interfaces.Introspection.__InputValue),
                    typeof(Introspection.Interfaces.Introspection.__EnumValue),
                    typeof(Introspection.Interfaces.Introspection.__Directive),
                    typeof(Introspection.Interfaces.Introspection.__DirectiveLocation),
                };
        private readonly IServiceProvider serviceProvider;

        public Schema(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override IGraphQlResult<IEnumerable<__Directive>> directives() =>
            Original.Resolve(types => types.DirectiveInformation).ConvertableList().As<DirectiveDefinition>();

        public override IGraphQlResult<__Type?> mutationType() =>
            Original.Resolve(types => types.Mutation).Convertable().As<GraphQlType>();

        public override IGraphQlResult<__Type> queryType() =>
            Original.Resolve(types => types.Query).Convertable().As<GraphQlType>();

        public override IGraphQlResult<__Type?> subscriptionType() =>
            Original.Resolve(types => types.Subscription).Convertable().As<GraphQlType>();

        public override IGraphQlResult<IEnumerable<__Type>> types() =>
            Original.Resolve(types => introspectionTypes.Union(types.TypeInformation)).ConvertableList().As<GraphQlType>();

    }
}