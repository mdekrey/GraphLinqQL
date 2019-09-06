import { GraphQLField, GraphQLArgument, GraphQLInterfaceType } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { getPropertyName } from "./getPropertyName";
import { getFieldName } from "./getFieldName";
import { getOutputTypeName } from "./getOutputTypeName";
import { getInputTypeName } from "./getInputTypeName";

export function generateInterface(object: GraphQLInterfaceType, options: Options) {
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
public interface ${typeName} : IGraphQlResolvable
{
    ${Object.keys(fields)
      .map(fieldName => fields[fieldName])
      .map(field => resultAbstractDeclaration(field, options)).join(`
    `)}
}
`;
}

function resultAbstractDeclaration(field: GraphQLField<any, any, { [key: string]: any }>, options: Options) {
  const propertyName = getPropertyName(field.name, options);
  const typeName = getOutputTypeName(field.type, options);
  const args = field.args.map(arg => resultAbstractDeclarationArg(arg, options)).join(", ");
  return `${
    field.description
      ? `
      /// <summary>
      /// ${field.description.split("\n").join("\n    /// ")}
      /// </summary>
      `
      : ``
  }IGraphQlResult${typeName && `<${typeName}>`} ${propertyName}(${args});`;
}

function resultAbstractDeclarationArg(arg: GraphQLArgument, options: Options) {
  const fieldName = getFieldName(arg.name, options);
  const inputTypeName = getInputTypeName(arg.type, options);
  return `${inputTypeName} ${fieldName}`;
}
