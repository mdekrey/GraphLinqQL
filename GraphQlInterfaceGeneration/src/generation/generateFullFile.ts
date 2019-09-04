import { Options } from "./Options";
import { GraphQLSchema } from "graphql";
import { generateTypes } from "./generateTypes";
import { generateTypeResolver } from "./generateTypeResolver";

export function generateFullFile(schema: GraphQLSchema, options: Options): string {
  const types = generateTypes(schema, options);
  return `${options.using.map(ns => `using ${ns};`).join(`
`)}${
    options.useNullabilityIndicator
      ? `
#nullable enable`
      : ""
  }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8605 // Unboxing a possibly null value.

namespace ${options.namespace}
{
    ${generateTypeResolver(schema, options)}
    ${types.split("\n").join(`
    `)}
}
`;
}
