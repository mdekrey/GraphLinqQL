import { GraphQLUnionType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
export function getUnionTypeName(outputType: GraphQLUnionType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
