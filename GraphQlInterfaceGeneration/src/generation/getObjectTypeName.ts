import { GraphQLObjectType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { useNullabilityIndicator } from "./options/useNullabilityIndicator";
export function getObjectTypeName(outputType: GraphQLObjectType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && useNullabilityIndicator(options) ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
