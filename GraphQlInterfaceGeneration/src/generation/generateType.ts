import {
  GraphQLObjectType,
  GraphQLField,
  GraphQLOutputType,
  isNonNullType,
  assertOutputType,
  isScalarType,
  isEnumType,
  isUnionType,
  isInterfaceType,
  isObjectType,
  isListType,
  GraphQLScalarType,
  GraphQLList,
  GraphQLInputType,
  GraphQLEnumType,
  GraphQLInterfaceType,
  GraphQLUnionType,
  isInputObjectType,
  GraphQLInputObjectType,
  GraphQLArgument
} from "graphql";
import { Options } from "./options";
import { neverEver } from "../utils/neverEver";

function getTypeName(name: string, options: Options) {
  // TODO - this must be imported
  return name;
}

function getPropertyName(name: string, options: Options) {
  // TODO - this must be imported
  return name;
}

function getFieldName(name: string, options: Options) {
  // TODO - this must be imported
  return name;
}

function getOutputTypeName(outputType: GraphQLOutputType, options: Options, nullable: boolean = true): string {
  // TODO - this must be imported
  // This is a bit awkward, as C# assumes not-nullable and GraphQL assumes nullable
  if (isNonNullType(outputType)) {
    if (!nullable) {
      throw new Error("Not nullable in an already not-nullable context! " + outputType.toString());
    }
    return getOutputTypeName(outputType.ofType, options, false);
  }

  if (isScalarType(outputType)) {
    return getScalarTypeName(outputType, options, nullable);
  } else if (isListType(outputType)) {
    return getListTypeName(outputType, options, nullable, getOutputTypeName);
  } else if (isEnumType(outputType)) {
    return getEnumTypeName(outputType, options, nullable);
  } else if (isInterfaceType(outputType)) {
    return getInterfaceTypeName(outputType, options, nullable);
  } else if (isUnionType(outputType)) {
    return getUnionTypeName(outputType, options, nullable);
  } else if (isObjectType(outputType)) {
    return getObjectTypeName(outputType, options, nullable);
  } else {
    return neverEver(outputType);
  }
}

function getScalarTypeName(type: GraphQLScalarType, options: Options, nullable: boolean): string {
  const targetType = options.scalarTypes[type.name];
  const nullabilityIndicator = nullable && (!targetType.csharpNullable || options.useNullabilityIndicator) ? "?" : "";
  return targetType.csharpType + nullabilityIndicator;
}

function getListTypeName<T extends GraphQLOutputType | GraphQLInputType>(
  outputType: GraphQLList<any>,
  options: Options,
  nullable: boolean,
  typeNameFunction: (type: T, options: Options) => string
): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return `IEnumerable<${typeNameFunction(outputType.ofType, options)}>${nullabilityIndicator}`;
}

function getEnumTypeName(outputType: GraphQLEnumType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}

function getInterfaceTypeName(outputType: GraphQLInterfaceType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}

function getUnionTypeName(outputType: GraphQLUnionType, options: Options, nullable: boolean): string {
  return "";
}

function getObjectTypeName(outputType: GraphQLObjectType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}

function getInputObjectTypeName(outputType: GraphQLInputObjectType, options: Options, nullable: boolean): string {
  const nullabilityIndicator = nullable && options.useNullabilityIndicator ? "?" : "";
  return getTypeName(outputType.name, options) + nullabilityIndicator;
}

function getInputTypeName(inputType: GraphQLInputType, options: Options, nullable: boolean = true): string {
  // This is a bit awkward, as C# assumes not-nullable and GraphQL assumes nullable
  if (isNonNullType(inputType)) {
    if (!nullable) {
      throw new Error("Not nullable in an already not-nullable context! " + inputType.toString());
    }
    return getOutputTypeName(inputType.ofType, options, false);
  }

  if (isScalarType(inputType)) {
    return getScalarTypeName(inputType, options, nullable);
  } else if (isListType(inputType)) {
    return getListTypeName(inputType, options, nullable, getOutputTypeName);
  } else if (isEnumType(inputType)) {
    return getEnumTypeName(inputType, options, nullable);
  } else if (isInterfaceType(inputType)) {
    return getInterfaceTypeName(inputType, options, nullable);
  } else if (isUnionType(inputType)) {
    return getUnionTypeName(inputType, options, nullable);
  } else if (isInputObjectType(inputType)) {
    return getInputObjectTypeName(inputType, options, nullable);
  } else {
    return neverEver(inputType);
  }
}

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
public abstract class ${typeName} ${interfaceDeclaration(object, options)}
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
  // TODO
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
