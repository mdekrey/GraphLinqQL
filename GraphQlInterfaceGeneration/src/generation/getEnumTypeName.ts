import { GraphQLEnumType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
export function getEnumTypeName(outputType: GraphQLEnumType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
