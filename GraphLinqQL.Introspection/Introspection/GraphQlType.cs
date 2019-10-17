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


        public override IGraphQlScalarResult<string?> description() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Description);

        public override IGraphQlObjectResult<IEnumerable<__EnumValue>?> enumValues(bool? includeDeprecated) =>
            Original.Join(typeInformation).Resolve((_, info) => info.EnumValues(includeDeprecated)).Nullable(_ => _.List(_ => _.AsContract<EnumValue>()));

        public override IGraphQlObjectResult<IEnumerable<__Field>?> fields(bool? includeDeprecated) =>
            Original.Join(typeInformation).Resolve((_, info) => info.Fields(includeDeprecated)).Nullable(_ => _.List(_ => _.AsContract<GraphQlField>()));

        public override IGraphQlObjectResult<IEnumerable<__InputValue>?> inputFields() =>
            Original.Join(typeInformation).Resolve((_, info) => info.InputFields).Nullable(_ => _.List(_ => _.AsContract<GraphQlInputField>()));

        public override IGraphQlObjectResult<IEnumerable<__Type>?> interfaces() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Interfaces).Nullable(_ => _.List(_ => _.AsContract<GraphQlType>()));

        public override IGraphQlScalarResult<__TypeKind> kind() =>
            Original.Join(typeInformation).Resolve((_, info) => ToInterfaceKind(info.Kind));

        public override IGraphQlScalarResult<string?> name() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Name);

        public override IGraphQlObjectResult<__Type?> ofType() =>
            Original.Join(typeInformation).Resolve((_, info) => info.OfType).Nullable(_ => _.AsContract<GraphQlType>());

        public override IGraphQlObjectResult<IEnumerable<__Type>?> possibleTypes() =>
            Original.Join(typeInformation).Resolve((_, info) => info.PossibleTypes).Nullable(_ => _.List(_ => _.AsContract<GraphQlType>()));

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