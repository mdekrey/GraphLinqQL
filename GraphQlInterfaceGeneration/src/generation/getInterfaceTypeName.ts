import { GraphQLInterfaceType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { useNullabilityIndicator } from "./options/useNullabilityIndicator";
export function getInterfaceTypeName(outputType: GraphQLInterfaceType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && useNullabilityIndicator(options) ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}
