import { GraphQLInputObjectType, GraphQLInputField } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { getPropertyName } from "./getPropertyName";
import { getInputTypeName } from "./getInputTypeName";

export function generateInputType(object: GraphQLInputObjectType, options: Options) {
  const typeName = getTypeName(object.name, options);

  const fields = object.getFields();
  return `${
    object.description
      ? `
/// <summary>
/// ${object.description.split("\n").join("\n/// ")}
/// </summary>`
      : `
`
  }
public class ${typeName}
{
    ${Object.keys(fields)
      .map(fieldName => fields[fieldName])
      .map(field => propertyDeclaration(field, options)).join(`
    `)}
}
`;
}

function propertyDeclaration(field: GraphQLInputField, options: Options) {
  const propertyName = getPropertyName(field.name, options);
  const typeName = getInputTypeName(field.type, options);
  // TODO - defaultValue
  return `${
    field.description
      ? `
      /// <summary>
      /// ${field.description.split("\n").join("\n    /// ")}
      /// </summary>
      `
      : ``
  }public ${typeName || "object"} ${propertyName} { get; set; }`;
}
