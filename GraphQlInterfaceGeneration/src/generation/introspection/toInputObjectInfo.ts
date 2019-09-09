import { Options } from "../Options";
import { GraphQLInputField, GraphQLArgument } from "graphql";
import { multilineString } from "./multilineString";
import { toIntrospectionType } from "./toIntrospectionType";
export function toInputObjectInfo(f: GraphQLInputField | GraphQLArgument, options: Options) {
  return `new GraphQlInputFieldInformation(name: "${f.name}", type: typeof(${toIntrospectionType(
    f.type,
    options
  )}), description: ${multilineString(f.description)}${
    f.defaultValue === undefined ? "" : `, defaultValue: ${multilineString("" + f.defaultValue)}`
  })`;
}
