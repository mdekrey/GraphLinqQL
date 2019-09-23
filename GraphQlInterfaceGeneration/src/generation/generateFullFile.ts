import { Options } from "./Options";
import { GraphQLSchema } from "graphql";
import { generateTypes } from "./generateTypes";
import { generateTypeResolver } from "./generateTypeResolver";
import { generateIntrospectionNamespace } from "./introspection/generateIntrospectionNamespace";
import { useNullabilityIndicator } from "./options/useNullabilityIndicator";

export function generateFullFile(schema: GraphQLSchema, options: Options): string {
  const types = generateTypes(schema, options);
  return `${options.using.map(ns => `using ${ns};`).join(`
`)}${
    useNullabilityIndicator(options)
      ? `
#nullable enable
#nullable disable warnings`
      : ""
  }
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
