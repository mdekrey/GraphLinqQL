import { Options } from "../Options";
import { GraphQLSchema } from "graphql";
import { generateTypeListing } from "./generateTypeListing";
import { generateTypeInfo } from "./generateTypeInfo";

export function generateIntrospectionNamespace(schema: GraphQLSchema, options: Options): string {
  return `
namespace Introspection
{
    ${generateTypeListing(schema, options).split("\n").join(`
    `)}${Object.keys(schema.getTypeMap())
    .map(typeName => schema.getType(typeName))
    .map(type => generateTypeInfo(type!, options))
    .join("\n")
    .split("\n").join(`
    `)}
}
`;
}
