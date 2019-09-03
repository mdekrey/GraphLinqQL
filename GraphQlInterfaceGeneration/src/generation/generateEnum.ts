import { GraphQLEnumType, GraphQLEnumValue } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { getEnumValueName } from "./getEnumValueName";

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
    `) + getEnumValueName(value.name, options)
  );
}
