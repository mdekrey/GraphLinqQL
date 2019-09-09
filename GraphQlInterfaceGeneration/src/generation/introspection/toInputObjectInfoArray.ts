import { Options } from "../Options";
import { GraphQLInputField, GraphQLArgument } from "graphql";
import { toInputObjectInfo } from "./toInputObjectInfo";
export function toInputObjectInfoArray(f: (GraphQLInputField | GraphQLArgument)[], options: Options) {
  return `new GraphQlInputFieldInformation[] {
                ${f.map(field => toInputObjectInfo(field, options)).join(`,
                `)}
            }`;
}
