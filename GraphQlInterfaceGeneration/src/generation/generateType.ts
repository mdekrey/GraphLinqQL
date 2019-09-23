import { GraphQLObjectType, GraphQLField, isNonNullType, GraphQLArgument } from "graphql";
import { Options } from "./Options";
import { getTypeName } from "./getTypeName";
import { getPropertyName } from "./getPropertyName";
import { getFieldName } from "./getFieldName";
import { getOutputTypeName } from "./getOutputTypeName";
import { getInputTypeName } from "./getInputTypeName";
import { toCsharpValue } from "./toCsharpValue";

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
public abstract class ${typeName} : IGraphQlResolvable${interfaceDeclaration(object, options)}
{
    private ${typeName}() { }
    ${Object.keys(fields)
      .map(fieldName => fields[fieldName])
      .map(field => resultAbstractDeclaration(field, options)).join(`
    `)}

    IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
    {
        switch (name)
        {
            case "__typename": return GraphQlConstantResult.Construct("${object.name}");
            ${Object.keys(fields)
              .map(fieldName => fields[fieldName])
              .map(field => fieldResolveQueryCase(field, options)).join(`
            `)}
            default: throw new ArgumentException("Unknown property " + name, "name");
        };
    }

    bool IGraphQlResolvable.IsType(string value)
    {
        return ${[object.name]
          .concat(object.getInterfaces().map(iface => iface.name))
          .map(n => `value == "${n}"`)
          .join(" || ")};
    }

    public abstract class GraphQlContract<T> : ${typeName}, IGraphQlAccepts<T>
    {
        public IGraphQlResultFactory<T> Original { get; set; }
        IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        Type IGraphQlAccepts.ModelType { get { return typeof(T); } }
    }
}
`;
}

function interfaceDeclaration(object: GraphQLObjectType, options: Options) {
  if (object.getInterfaces().length) {
    return (
      ", " +
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
  const obsoleteAttr = field.isDeprecated
    ? `[Obsolete(${field.deprecationReason ? JSON.stringify(field.deprecationReason) : ""})]
    `
    : ``;
  return `${
    field.description
      ? `
    /// <summary>
    /// ${field.description.split("\n").join("\n    /// ")}
    /// </summary>
    `
      : ``
  }${obsoleteAttr}public abstract IGraphQlResult${typeName && `<${typeName}>`} ${propertyName}(${args});`;
}

function resultAbstractDeclarationArg(arg: GraphQLArgument, options: Options) {
  const fieldName = getFieldName(arg.name, options);
  const inputTypeName = getInputTypeName(arg.type, options);
  return `${inputTypeName} ${fieldName}`;
}

function fieldResolveQueryCase(field: GraphQLField<any, any, { [key: string]: any }>, options: Options) {
  const propertyName = getPropertyName(field.name, options);
  const args = field.args.map(arg => fieldResolveQueryCaseArg(arg, options));
  return `case "${field.name}": return this.${propertyName}(${
    args.length
      ? `
                ${args.join(`,
                `)}`
      : ""
  });`;
}

function fieldResolveQueryCaseArg(arg: GraphQLArgument, options: Options) {
  const fieldName = getFieldName(arg.name, options);
  const inputTypeName = getInputTypeName(arg.type, options);
  const nullable = arg.defaultValue || !isNonNullType(arg.type);
  const defaultValue = arg.defaultValue;

  const getValue = nullable
    ? `(parameters.HasParameter("${arg.name}") ? parameters.GetParameter<${inputTypeName}>("${arg.name}") : null)`
    : `parameters.GetParameter<${inputTypeName}>("${arg.name}")`;
  const defaultValueExpression = defaultValue ? ` ?? ${toCsharpValue(arg.defaultValue, arg.type, options)}` : "";
  return `${fieldName}: ${getValue}${defaultValueExpression}`;
}
