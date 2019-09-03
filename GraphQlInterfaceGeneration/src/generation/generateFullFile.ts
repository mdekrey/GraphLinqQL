import { Options } from "./Options";
import { GraphQLSchema } from "graphql";
import { generateTypes } from "./generateTypes";

export function generateFullFile(schema: GraphQLSchema, options: Options): string {
  const types = generateTypes(schema, options);
  return `${options.using.map(ns => `using ${ns};`).join(`
`)}${
    options.useNullabilityIndicator
      ? `
#nullable enable`
      : ""
  }

namespace ${options.namespace}
{
    ${types.split("\n").join(`
    `)}
}
`;
}
