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
  GraphQLInputObjectType,
  GraphQLOutputType,
  GraphQLInputType,
  GraphQLInputField,
  GraphQLArgument,
  isListType,
  isNonNullType
} from "graphql";
import { getTypeName } from "../getTypeName";
import { neverEver } from "../../utils/neverEver";
import { toCsharpValue } from "../toCsharpValue";

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

const inputFieldsList: TypeKindSwitch<string> = {
  InputObject: (obj, options) =>
    toInputObjectInfoArray(Object.keys(obj.getFields()).map(fieldName => obj.getFields()[fieldName]), options) +
    `.ToImmutableList()`
};

const enumValuesList: TypeKindSwitch<string> = {
  Enum: (obj, options) =>
    `
        enumValues.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
    private static readonly IReadOnlyList<GraphQlEnumValueInformation> enumValues = new GraphQlEnumValueInformation[] {
        ${obj
          .getValues()
          .map(
            enumValue =>
              `new GraphQlEnumValueInformation(name: "${enumValue.name}", description: ${multilineString(
                enumValue.description
              )}, isDeprecated: ${enumValue.isDeprecated}, deprecationReason: ${multilineString(
                enumValue.deprecationReason
              )})`
          ).join(`,
        `)}
    }.ToImmutableList()`
};

const fieldList: TypeKindSwitch<string> = {
  Object: (obj, options) =>
    `
        fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
    private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
        ${Object.keys(obj.getFields())
          .map(fieldName => obj.getFields()[fieldName])
          .map(
            field =>
              `new GraphQlFieldInformation(
            type: typeof(${toIntrospectionType(field.type, options)}),
            name: "${field.name}",
            args: ${toInputObjectInfoArray(field.args, options)},
            description: ${multilineString(field.description)},
            isDeprecated: ${field.isDeprecated},
            deprecationReason: ${multilineString(field.deprecationReason)})`
          ).join(`,
        `)}
    }.ToImmutableList()`
};

export function generateTypeInfo(type: GraphQLNamedType, options: Options) {
  return `
public class ${getTypeName(type.name, options)} : IGraphQlTypeInformation
{
    public string Name => "${type.name}";
    public string? Description => ${type.description ? multilineString(type.description) : null};
    public TypeKind Kind => TypeKind.${typeSwitch(type, options, typeKindEnum, neverEver as any)};
    public Type? OfType => null;
    public IReadOnlyList<Type>? Interfaces => ${typeSwitch(type, options, interfacesList, () => "null")};
    public IReadOnlyList<Type>? PossibleTypes => ${typeSwitch(type, options, possibleTypesList, () => "null")};

    public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get; } = ${typeSwitch(
      type,
      options,
      inputFieldsList,
      () => "null"
    )};

    public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated) => ${typeSwitch(
      type,
      options,
      enumValuesList,
      () => "null"
    )};
    public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated) => ${typeSwitch(
      type,
      options,
      fieldList,
      () => "null"
    )};
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

function multilineString(string: string | null | undefined) {
  return string === null || string === undefined ? null : `@"${string.replace(/"/g, '""')}"`;
}

function toIntrospectionType(type: GraphQLInputType | GraphQLOutputType, options: Options): string {
  if (isListType(type)) {
    return `ListTypeInformation<${toIntrospectionType(type.ofType, options)}>`;
  } else if (isNonNullType(type)) {
    return `NonNullTypeInformation<${toIntrospectionType(type.ofType, options)}>`;
  } else {
    return getTypeName(type.name, options);
  }
}

function toInputObjectInfo(f: GraphQLInputField | GraphQLArgument, options: Options) {
  return `new GraphQlInputFieldInformation(name: "${f.name}", type: typeof(${toIntrospectionType(
    f.type,
    options
  )}), description: ${multilineString(f.description)}${
    f.defaultValue === undefined ? "" : `, defaultValue: ${multilineString("" + f.defaultValue)}`
  })`;
}

function toInputObjectInfoArray(f: (GraphQLInputField | GraphQLArgument)[], options: Options) {
  return `new GraphQlInputFieldInformation[] {
                ${f.map(field => toInputObjectInfo(field, options)).join(`,
                `)}
            }`;
}
