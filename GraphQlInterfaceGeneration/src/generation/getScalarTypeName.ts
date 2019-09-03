import { GraphQLScalarType } from "graphql";
import { Options } from "./Options";
export function getScalarTypeName(type: GraphQLScalarType, options: Options, nullable: boolean): string {
  const targetType = options.scalarTypes[type.name] || (type.name === "ID" ? options.scalarTypes["String"] : null);
  const nullabilityIndicator = nullable && (!targetType.csharpNullable || options.useNullabilityIndicator) ? "?" : "";
  return targetType.csharpType + nullabilityIndicator;
}
