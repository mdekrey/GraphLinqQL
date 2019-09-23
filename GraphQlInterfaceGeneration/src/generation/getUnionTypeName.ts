import { GraphQLUnionType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { useNullabilityIndicator } from "./options/useNullabilityIndicator";
export function getUnionTypeName(outputType: GraphQLUnionType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && useNullabilityIndicator(options) ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
