import { isNonNullType, GraphQLInputType, isScalarType, isEnumType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { getInputTypeName } from "./getInputTypeName";
import { getEnumValueName } from "./getEnumValueName";
export function toCsharpValue(value: any, type: GraphQLInputType, options: Options) {
  const inputTypeName = getInputTypeName(type, options);
  // whether null or not-null, the default value when provided is not-null
  type = isNonNullType(type) ? type.ofType : type;
  const stringified = JSON.stringify(value);
  if (isScalarType(type)) {
    if (type.name === "String" || type.name === "Int" || type.name === "Float" || type.name === "Boolean") {
      return stringified;
    }
  } else if (isEnumType(type)) {
    return `${options.namespace}.${getTypeName(type.name, options)}.${getEnumValueName(value, options)}`;
  }
  return `${options.deserializer}<${inputTypeName}>(${JSON.stringify(stringified)})`;
}
