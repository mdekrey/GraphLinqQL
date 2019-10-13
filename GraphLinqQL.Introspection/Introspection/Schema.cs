﻿using System;
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

        public override IGraphQlObjectResult<IEnumerable<__Directive>> directives(FieldContext fieldContext) =>
            Original.Resolve(types => types.DirectiveInformation).List(_ => _.AsContract<DirectiveDefinition>());

        public override IGraphQlObjectResult<__Type?> mutationType(FieldContext fieldContext) =>
            Original.Resolve(types => types.Mutation).Nullable(_ => _.AsContract<GraphQlType>());

        public override IGraphQlObjectResult<__Type> queryType(FieldContext fieldContext) =>
            Original.Resolve(types => types.Query).AsContract<GraphQlType>();

        public override IGraphQlObjectResult<__Type?> subscriptionType(FieldContext fieldContext) =>
            Original.Resolve(types => types.Subscription).Nullable(_ => _.AsContract<GraphQlType>());

        public override IGraphQlObjectResult<IEnumerable<__Type>> types(FieldContext fieldContext) =>
            Original.Resolve(types => introspectionTypes.Union(types.TypeInformation)).List(_ => _.AsContract<GraphQlType>());

    }
}