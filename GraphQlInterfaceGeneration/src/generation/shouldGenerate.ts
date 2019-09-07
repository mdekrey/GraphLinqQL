import { GraphQLNamedType } from "graphql";
import { Options } from "./Options";

export function shouldGenerate(type: GraphQLNamedType, options: Options): boolean {
  return (
    (options.introspection || !type.name.startsWith("__")) && type.name !== "ID" && !options.scalarTypes[type.name]
  );
}
