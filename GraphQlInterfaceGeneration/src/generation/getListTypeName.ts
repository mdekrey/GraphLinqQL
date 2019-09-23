import { GraphQLOutputType, GraphQLList, GraphQLInputType } from "graphql";
import { Options } from "./Options";
import { useNullabilityIndicator } from "./options/useNullabilityIndicator";
export function getListTypeName<T extends GraphQLOutputType | GraphQLInputType>(
  outputType: GraphQLList<any>,
  options: Options,
  nullable: boolean,
  typeNameFunction: (type: T, options: Options) => string
): string {
  const nullabilityIndicator = nullable && useNullabilityIndicator(options) ? "?" : "";
  const genericType = typeNameFunction(outputType.ofType, options);
  if (!genericType) {
    return `IEnumerable${nullabilityIndicator}`;
  }
  return `IEnumerable<${genericType}>${nullabilityIndicator}`;
}
