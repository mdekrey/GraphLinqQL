import { GraphQLNamedType } from "graphql";
import { Options } from "./options";

export function shouldGenerate(type: GraphQLNamedType, options: Options): boolean {
  return !type.name.startsWith("__") && !options.scalarTypes[type.name];
}
