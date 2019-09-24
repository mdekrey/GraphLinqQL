using System;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
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

        public override IGraphQlResult<IEnumerable<__Directive>> directives() =>
            Original.Resolve(types => types.DirectiveInformation).List(_ => _.As<DirectiveDefinition>());

        public override IGraphQlResult<__Type?> mutationType() =>
            Original.Resolve(types => types.Mutation).Nullable(_ => _.As<GraphQlType>());

        public override IGraphQlResult<__Type> queryType() =>
            Original.Resolve(types => types.Query).As<GraphQlType>();

        public override IGraphQlResult<__Type?> subscriptionType() =>
            Original.Resolve(types => types.Subscription).Nullable(_ => _.As<GraphQlType>());

        public override IGraphQlResult<IEnumerable<__Type>> types() =>
            Original.Resolve(types => introspectionTypes.Union(types.TypeInformation)).List(_ => _.As<GraphQlType>());

    }
}