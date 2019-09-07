import { Options } from "../Options";
import {
  GraphQLNamedType,
  isScalarType,
  isObjectType,
  isUnionType,
  isEnumType,
  isInterfaceType,
  isInputObjectType,
  GraphQLScalarType,
  GraphQLObjectType,
  GraphQLInterfaceType,
  GraphQLUnionType,
  GraphQLEnumType,
  GraphQLInputObjectType
} from "graphql";
import { getTypeName } from "../getTypeName";
import { neverEver } from "../../utils/neverEver";

type TypeKindMap = {
  Scalar: GraphQLScalarType;
  Object: GraphQLObjectType;
  Interface: GraphQLInterfaceType;
  Union: GraphQLUnionType;
  Enum: GraphQLEnumType;
  InputObject: GraphQLInputObjectType;
};

const typeKindTest: { [P in keyof TypeKindMap]: (t: GraphQLNamedType) => t is TypeKindMap[P] } = {
  Scalar: isScalarType,
  Object: isObjectType,
  Interface: isInterfaceType,
  Union: isUnionType,
  Enum: isEnumType,
  InputObject: isInputObjectType
};

type TypeKindSwitch<T> = { [P in keyof TypeKindMap]?: (t: TypeKindMap[P], options: Options) => T };
const typeKindEnum: TypeKindSwitch<string> = {
  Scalar: () => "Scalar",
  Object: () => "Object",
  Interface: () => "Interface",
  Union: () => "Union",
  Enum: () => "Enum",
  InputObject: () => "InputObject"
};

const interfacesList: TypeKindSwitch<string> = {
  Object: (obj, options) =>
    `new Type[] {
        ${obj
          .getInterfaces()
          .map(iface => getTypeName(iface.name, options))
          .map(v => `typeof(${v})`).join(`,
        `)}
    }.ToImmutableList()`
};

const possibleTypesList: TypeKindSwitch<string> = {
  Union: (obj, options) =>
    `new Type[] {
        ${obj
          .getTypes()
          .map(type => getTypeName(type.name, options))
          .map(v => `typeof(${v})`).join(`,
        `)}
    }.ToImmutableList()`
};

export function generateTypeInfo(type: GraphQLNamedType, options: Options) {
  return `
public class ${getTypeName(type.name, options)} : IGraphQlTypeInformation
{
    public string Name => "${type.name}";
    public string? Description => ${type.description ? `@"${type.description.replace(/"/g, '""')}"` : null};
    public TypeKind Kind => TypeKind.${typeSwitch(type, options, typeKindEnum, neverEver as any)};
    public Type? OfType => null;
    public IReadOnlyList<Type>? Interfaces => ${typeSwitch(type, options, interfacesList, () => "null")};
    public IReadOnlyList<Type>? PossibleTypes => ${typeSwitch(type, options, possibleTypesList, () => "null")};

    public IReadOnlyList<GraphQlInputFieldInformation> InputFields => throw new NotImplementedException();
    public IReadOnlyList<GraphQlEnumValueInformation> EnumValues(bool? includeDeprecated) => throw new NotImplementedException();
    public IReadOnlyList<GraphQlFieldInformation> Fields(bool? includeDeprecated) => throw new NotImplementedException();
}`;
}

function typeSwitch<T>(
  type: GraphQLNamedType,
  options: Options,
  target: TypeKindSwitch<T>,
  otherwise: (type: GraphQLNamedType) => T
) {
  const key = Object.keys(target).find(
    k => typeKindTest[k as keyof TypeKindMap] && typeKindTest[k as keyof TypeKindMap](type)
  ) as (keyof TypeKindMap | undefined);
  if (key) {
    return target[key]!(type as any, options);
  } else {
    return otherwise(type);
  }
}
