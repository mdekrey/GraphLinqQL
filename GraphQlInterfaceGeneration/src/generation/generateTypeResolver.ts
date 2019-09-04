import { Options } from "./Options";
import { GraphQLSchema, GraphQLInputObjectType, isInputObjectType } from "graphql";
import { getInputTypeName } from "./getInputTypeName";

export function generateTypeResolver(schema: GraphQLSchema, options: Options) {
  const inputObjectTypes = Object.keys(schema.getTypeMap())
    .map(typeName => schema.getType(typeName))
    .filter((t): t is GraphQLInputObjectType => (t ? isInputObjectType(t) : false));
  const hasId = Boolean(options.scalarTypes["ID"]);
  return `
public class TypeResolver : IGraphQlTypeResolver
{
    public Type Resolve(string name)
    {
        switch (name)
        {
            ${hasId ? "" : scalarType("ID", "String")}
            ${Object.keys(options.scalarTypes).map(t => scalarType(t, t)).join(`
            `)}
            ${inputObjectTypes.map(
              input => `case "${input.name}":
                return typeof(${getInputTypeName(input, options, false)});`
            ).join(`
            `)}
            default:
                throw new ArgumentException("Unknown type " + name, nameof(name));
        }
    }
}`;
  function scalarType(label: string, t: string) {
    return `case "${label}":
                return typeof(${`${options.scalarTypes[t].csharpType}${
                  options.scalarTypes[t].csharpNullable ? "" : "?"
                }`});`;
  }
}
