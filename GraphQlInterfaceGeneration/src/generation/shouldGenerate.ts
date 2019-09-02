import { GraphQLNamedType } from "graphql";

export function shouldGenerate(type: GraphQLNamedType) {
  return !type.name.startsWith("__");
}
