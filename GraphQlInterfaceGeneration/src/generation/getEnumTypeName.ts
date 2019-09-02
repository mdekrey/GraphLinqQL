import { GraphQLEnumType } from "graphql";
import { Options } from "./options";
import { getTypeName } from "./getTypeName";
export function getEnumTypeName(outputType: GraphQLEnumType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
