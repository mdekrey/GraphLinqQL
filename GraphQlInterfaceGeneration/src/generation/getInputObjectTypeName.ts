import { GraphQLInputObjectType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
export function getInputObjectTypeName(
  outputType: GraphQLInputObjectType,
  options: Options,
  nullable: boolean
): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
