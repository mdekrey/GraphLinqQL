import { GraphQLInputObjectType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { useNullabilityIndicator } from "./options/useNullabilityIndicator";
export function getInputObjectTypeName(
  outputType: GraphQLInputObjectType,
  options: Options,
  nullable: boolean
): string {
  const nullabilityIndicator = nullable && useNullabilityIndicator(options) ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
