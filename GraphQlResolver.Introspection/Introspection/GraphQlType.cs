﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    class GraphQlType : Interfaces.__Type.GraphQlContract<Type>
    {
        private readonly IServiceProvider serviceProvider;

        public GraphQlType(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            typeInformation = GraphQlJoin.JoinSingle<Type, IGraphQlTypeInformation>((type) => serviceProvider.MaybeInstantiate(type)!);
        }

        private readonly GraphQlJoin<Type, IGraphQlTypeInformation> typeInformation; 


        public override IGraphQlResult<string?> description() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Description);

        public override IGraphQlResult<IEnumerable<__EnumValue>?> enumValues(bool? includeDeprecated) =>
            Original.Join(typeInformation).Resolve((_, info) => info.EnumValues(includeDeprecated)).ConvertableList().As<EnumValue>();

        public override IGraphQlResult<IEnumerable<__Field>?> fields(bool? includeDeprecated) =>
            Original.Join(typeInformation).Resolve((_, info) => info.Fields(includeDeprecated)).ConvertableList().As<GraphQlField>();

        public override IGraphQlResult<IEnumerable<__InputValue>?> inputFields() =>
            Original.Join(typeInformation).Resolve((_, info) => info.InputFields).ConvertableList().As<GraphQlInputField>();

        public override IGraphQlResult<IEnumerable<__Type>?> interfaces() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Interfaces).ConvertableList().As<GraphQlType>();

        public override IGraphQlResult<__TypeKind> kind() =>
            Original.Join(typeInformation).Resolve((_, info) => ToInterfaceKind(info.Kind));

        public override IGraphQlResult<string?> name() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Name);

        public override IGraphQlResult<__Type?> ofType() =>
            Original.Join(typeInformation).Resolve((_, info) => info.OfType).Convertable().As<GraphQlType>();

        public override IGraphQlResult<IEnumerable<__Type>?> possibleTypes() =>
            Original.Join(typeInformation).Resolve((_, info) => info.PossibleTypes).ConvertableList().As<GraphQlType>();

        private __TypeKind ToInterfaceKind(TypeKind kind)
        {
            switch (kind)
            {
                case TypeKind.Scalar:
                    return __TypeKind.SCALAR;
                case TypeKind.Object:
                    return __TypeKind.OBJECT;
                case TypeKind.Interface:
                    return __TypeKind.INTERFACE;
                case TypeKind.Union:
                    return __TypeKind.UNION;
                case TypeKind.Enum:
                    return __TypeKind.ENUM;
                case TypeKind.InputObject:
                    return __TypeKind.INPUT_OBJECT;
                case TypeKind.List:
                    return __TypeKind.LIST;
                case TypeKind.NonNull:
                    return __TypeKind.NON_NULL;
                default:
                    throw new NotSupportedException();
            }
        }

    }
}