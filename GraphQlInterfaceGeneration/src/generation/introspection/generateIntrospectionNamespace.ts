import { Options } from "../Options";
import { GraphQLSchema, GraphQLNamedType } from "graphql";
import { generateTypeListing } from "./generateTypeListing";
import { getTypeName } from "../getTypeName";

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

export function generateTypeInfo(type: GraphQLNamedType, options: Options) {
  return `
public class ${getTypeName(type.name, options)} : IGraphQlTypeInformation
{
    public string Name => "${type.name}";
}`;
}
