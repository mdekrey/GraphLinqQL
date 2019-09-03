import { GraphQLOutputType, GraphQLList, GraphQLInputType } from "graphql";
import { Options } from "./Options";
export function getListTypeName<T extends GraphQLOutputType | GraphQLInputType>(
  outputType: GraphQLList<any>,
  options: Options,
  nullable: boolean,
  typeNameFunction: (type: T, options: Options) => string
): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return `IEnumerable<${typeNameFunction(outputType.ofType, options)}>${nullabilityIndicator}`;
}
