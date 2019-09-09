import { Options } from "../Options";
import { GraphQLOutputType, GraphQLInputType, isListType, isNonNullType } from "graphql";
import { getTypeName } from "../getTypeName";
export function toIntrospectionType(type: GraphQLInputType | GraphQLOutputType, options: Options): string {
  if (isListType(type)) {
    return `ListTypeInformation<${toIntrospectionType(type.ofType, options)}>`;
  } else if (isNonNullType(type)) {
    return `NonNullTypeInformation<${toIntrospectionType(type.ofType, options)}>`;
  } else {
    return getTypeName(type.name, options);
  }
}
