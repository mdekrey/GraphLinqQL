import { Options } from "./Options";
import { GraphQLSchema } from "graphql";
import { generateTypes } from "./generateTypes";
import { generateTypeResolver } from "./generateTypeResolver";
import { generateIntrospectionNamespace } from "./introspection/generateIntrospectionNamespace";

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
#pragma warning disable CS0618 // Type or member is obsolete. (Don't check these in the generated file because it only calls itself.)

namespace ${options.namespace}
{
    ${generateTypeResolver(schema, options).split("\n").join(`
    `)}
    ${types.split("\n").join(`
    `)}
    ${generateIntrospectionNamespace(schema, options).split("\n").join(`
    `)}
}
`;
}
