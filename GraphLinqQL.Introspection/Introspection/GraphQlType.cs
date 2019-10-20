using System;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    class GraphQlType : Interfaces.__Type.GraphQlContract<Type>
    {
        public GraphQlType(IGraphQlServiceProvider serviceProvider)
        {
            typeInformation = GraphQlJoin.JoinSingle<Type, IGraphQlTypeInformation>((type) => serviceProvider.TryGetTypeInformation(type)!);
        }

        private readonly GraphQlJoin<Type, IGraphQlTypeInformation> typeInformation; 


        public override IGraphQlScalarResult<string?> Description() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Description);

        public override IGraphQlObjectResult<IEnumerable<__EnumValue>?> EnumValues(bool? includeDeprecated) =>
            Original.Join(typeInformation).Resolve((_, info) => info.EnumValues(includeDeprecated)).Nullable(_ => _.List(_ => _.AsContract<EnumValue>()));

        public override IGraphQlObjectResult<IEnumerable<__Field>?> Fields(bool? includeDeprecated) =>
            Original.Join(typeInformation).Resolve((_, info) => info.Fields(includeDeprecated)).Nullable(_ => _.List(_ => _.AsContract<GraphQlField>()));

        public override IGraphQlObjectResult<IEnumerable<__InputValue>?> InputFields() =>
            Original.Join(typeInformation).Resolve((_, info) => info.InputFields).Nullable(_ => _.List(_ => _.AsContract<GraphQlInputField>()));

        public override IGraphQlObjectResult<IEnumerable<__Type>?> Interfaces() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Interfaces).Nullable(_ => _.List(_ => _.AsContract<GraphQlType>()));

        public override IGraphQlScalarResult<__TypeKind> Kind() =>
            Original.Join(typeInformation).Resolve((_, info) => ToInterfaceKind(info.Kind));

        public override IGraphQlScalarResult<string?> Name() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Name);

        public override IGraphQlObjectResult<__Type?> OfType() =>
            Original.Join(typeInformation).Resolve((_, info) => info.OfType).Nullable(_ => _.AsContract<GraphQlType>());

        public override IGraphQlObjectResult<IEnumerable<__Type>?> PossibleTypes() =>
            Original.Join(typeInformation).Resolve((_, info) => info.PossibleTypes).Nullable(_ => _.List(_ => _.AsContract<GraphQlType>()));

        private __TypeKind ToInterfaceKind(TypeKind kind)
        {
            switch (kind)
            {
                case TypeKind.Scalar:
                    return __TypeKind.Scalar;
                case TypeKind.Object:
                    return __TypeKind.Object;
                case TypeKind.Interface:
                    return __TypeKind.Interface;
                case TypeKind.Union:
                    return __TypeKind.Union;
                case TypeKind.Enum:
                    return __TypeKind.Enum;
                case TypeKind.InputObject:
                    return __TypeKind.InputObject;
                case TypeKind.List:
                    return __TypeKind.List;
                case TypeKind.NonNull:
                    return __TypeKind.NonNull;
                default:
                    throw new NotSupportedException();
            }
        }

    }
}