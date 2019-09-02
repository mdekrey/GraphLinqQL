import { GraphQLObjectType, GraphQLField, isNonNullType, assertOutputType, GraphQLArgument } from "graphql";
import { Options } from "./options";
import { getTypeName } from "./getTypeName";
import { getPropertyName } from "./getPropertyName";
import { getFieldName } from "./getFieldName";
import { getOutputTypeName } from "./getOutputTypeName";
import { getInputTypeName } from "./getInputTypeName";

export function generateType(object: GraphQLObjectType, options: Options) {
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
public abstract class ${typeName}${interfaceDeclaration(object, options)}
{
    private ${typeName}() { }
    ${Object.keys(fields)
      .map(fieldName => fields[fieldName])
      .map(field => resultAbstractDeclaration(field, options)).join(`
    `)}

    IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IDictionary<string, object> parameters) =>
        name switch
        {
            ${Object.keys(fields)
              .map(fieldName => fields[fieldName])
              .map(field => fieldResolveQueryCase(field, options)).join(`
            `)}
            _ => throw new ArgumentException("Unknown property " + name, nameof(name))
        };

    bool IGraphQlResolvable.IsType(string value) =>
      value == "${object.name /* Yes, the actual type name goes here */}";

    public abstract class GraphQlContract<T> : ${typeName}, IGraphQlAccepts<T>
    {
#nullable disable
        public IGraphQlResultFactory<T> Original { get; set; }
#nullable restore

        IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        Type IGraphQlAccepts.ModelType => typeof(T);
    }
}
`;
}

function interfaceDeclaration(object: GraphQLObjectType, options: Options) {
  if (object.getInterfaces().length) {
    return (
      " : " +
      object
        .getInterfaces()
        .map(iface => getTypeName(iface.name, options))
        .join(", ")
    );
  }
  return "";
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
  }public abstract IGraphQlResult${typeName && `<${typeName}>`} ${propertyName}(${args});`;
}

function resultAbstractDeclarationArg(arg: GraphQLArgument, options: Options) {
  const fieldName = getFieldName(arg.name, options);
  const inputTypeName = getInputTypeName(arg.type, options);
  return `${inputTypeName} ${fieldName}`;
}

function fieldResolveQueryCase(field: GraphQLField<any, any, { [key: string]: any }>, options: Options) {
  const propertyName = getPropertyName(field.name, options);
  const args = field.args.map(arg => fieldResolveQueryCaseArg(arg, options));
  return `"${field.name}" => ${propertyName}(${
    args.length
      ? `
                ${args.join(`,
                `)}`
      : ""
  }),`;
}

function fieldResolveQueryCaseArg(arg: GraphQLArgument, options: Options) {
  const fieldName = getFieldName(arg.name, options);
  const inputTypeName = getInputTypeName(arg.type, options);
  const nullable = arg.defaultValue || !isNonNullType(arg.type);
  const defaultValue = arg.defaultValue;

  // TODO - other type conversions
  return `${fieldName}: (parameters.TryGetValue("${arg.name}", out var ${fieldName}) ? (${inputTypeName})${fieldName} : null)`;
}
