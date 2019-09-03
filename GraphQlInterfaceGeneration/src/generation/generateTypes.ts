import { Options } from "./Options";
import {
  GraphQLSchema,
  GraphQLNamedType,
  isObjectType,
  isInterfaceType,
  isScalarType,
  isEnumType,
  isUnionType,
  isInputObjectType
} from "graphql";
import { shouldGenerate } from "./shouldGenerate";
import { generateType } from "./generateType";
import { generateInterface } from "./generateInterface";
import { neverEver } from "../utils/neverEver";
import { getTypeName } from "./getTypeName";
import { generateEnum } from "./generateEnum";
import { generateInputType } from "./generateInputType";

export function generateTypes(schema: GraphQLSchema, options: Options): string {
  return Object.keys(schema.getTypeMap())
    .map(typeName => schema.getType(typeName))
    .filter((t): t is GraphQLNamedType => (t ? shouldGenerate(t, options) : false))
    .map(type => {
      if (isObjectType(type)) {
        return generateType(type, options);
      } else if (isInputObjectType(type)) {
        return generateInputType(type, options);
      } else if (isInterfaceType(type)) {
        return generateInterface(type, options);
      } else if (isEnumType(type)) {
        return generateEnum(type, options);
      } else if (isUnionType(type)) {
        return `// Union: ${getTypeName(type.name, options)}`;
      } else if (isScalarType(type)) {
        return `// Scalar: ${getTypeName(type.name, options)}`;
      } else {
        return neverEver(type);
      }
    })
    .join("\n");
}
