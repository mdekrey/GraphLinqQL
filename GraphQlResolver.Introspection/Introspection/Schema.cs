using System;
using System.Collections.Generic;
using System.Linq;
using GraphQlResolver.Introspection.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver.Introspection
{
    public class Schema : Interfaces.__Schema.GraphQlContract<IGraphQlTypeListing>
    {
        private readonly IServiceProvider serviceProvider;

        public Schema(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private IGraphQlTypeInformation? MaybeInstantiate(Type? t)
        {
            return t == null ? null
                : (IGraphQlTypeInformation)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, t);
        }

        private IGraphQlTypeInformation Instantiate(Type t)
        {
            return (IGraphQlTypeInformation)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, t);
        }

        public override IGraphQlResult<IEnumerable<__Directive>> directives()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<__Type?> mutationType() =>
            Original.Resolve(types => MaybeInstantiate(types.Mutation)).Convertable().As<GraphQlType>();

        public override IGraphQlResult<__Type> queryType() =>
            Original.Resolve(types => MaybeInstantiate(types.Query)).Convertable().As<GraphQlType>();

        public override IGraphQlResult<__Type?> subscriptionType() =>
            Original.Resolve(types => MaybeInstantiate(types.Subscription)).Convertable().As<GraphQlType>();

        public override IGraphQlResult<IEnumerable<__Type>> types() =>
            // TODO - include introspection types
            Original.Resolve(types => types.TypeInformation.Select(Instantiate)).ConvertableList().As<GraphQlType>();

    }
}