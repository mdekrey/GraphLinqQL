import { GraphQLOutputType, GraphQLList, GraphQLInputType } from "graphql";
import { Options } from "./Options";
export function getListTypeName<T extends GraphQLOutputType | GraphQLInputType>(
  outputType: GraphQLList<any>,
  options: Options,
  nullable: boolean,
  typeNameFunction: (type: T, options: Options) => string
): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  const genericType = typeNameFunction(outputType.ofType, options);
  if (!genericType) {
    return `IEnumerable${nullabilityIndicator}`;
  }
  return `IEnumerable<${genericType}>${nullabilityIndicator}`;
}
