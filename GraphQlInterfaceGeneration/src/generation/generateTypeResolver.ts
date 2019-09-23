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
            ${hasId ? "" : scalarTypeCase("ID", "String")}
            ${Object.keys(options.scalarTypes).map(t => scalarTypeCase(t, t)).join(`
            `)}
            ${inputObjectTypes.map(
              input => `case "${input.name}":
                return typeof(${getInputTypeName(input, options, false)});`
            ).join(`
            `)}
            default:
                throw new ArgumentException("Unknown type " + name, "name");
        }
    }
}`;
  function scalarTypeCase(label: string, t: string) {
    return `case "${label}":
                return typeof(${scalarType(t)});`;
  }

  function scalarType(t: string): string {
    if (t == "ID" && !options.scalarTypes[t]) {
      return scalarType("String");
    }
    return `${options.scalarTypes[t].csharpType}${options.scalarTypes[t].csharpNullable ? "" : "?"}`;
  }
}
