import { GraphQLEnumType, GraphQLEnumValue } from "graphql";
import { Options } from "./options";
import { getTypeName } from "./getTypeName";

export function generateEnum(object: GraphQLEnumType, options: Options) {
  const typeName = getTypeName(object.name, options);

  return `${
    object.description
      ? `
/// <summary>
/// ${object.description.split("\n").join("\n/// ")}
/// </summary>`
      : `
`
  }
public enum ${typeName}
{
    ${object.getValues().map(value => generateEnumValue(value, options)).join(`,
    `)}
}
`;
}

function generateEnumValue(value: GraphQLEnumValue, options: Options) {
  return (
    (value.description &&
      `
    /// <summary>
    /// ${value.description.split("\n").join("\n    /// ")}
    /// </summary>
    `) + value.name
  );
}