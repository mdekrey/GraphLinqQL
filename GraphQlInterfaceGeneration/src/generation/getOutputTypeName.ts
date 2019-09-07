import {
  GraphQLOutputType,
  isNonNullType,
  isScalarType,
  isEnumType,
  isUnionType,
  isInterfaceType,
  isObjectType,
  isListType
} from "graphql";
import { Options } from "./Options";
import { neverEver } from "../utils/neverEver";
import { getObjectTypeName } from "./getObjectTypeName";
import { getUnionTypeName } from "./getUnionTypeName";
import { getInterfaceTypeName } from "./getInterfaceTypeName";
import { getEnumTypeName } from "./getEnumTypeName";
import { getListTypeName } from "./getListTypeName";
import { getScalarTypeName } from "./getScalarTypeName";
export function getOutputTypeName(outputType: GraphQLOutputType, options: Options, nullable: boolean = true): string {
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
    return ""; // TODO - if Unions are supported for output types, use it here //getUnionTypeName(outputType, options, nullable);
  } else if (isObjectType(outputType)) {
    return getObjectTypeName(outputType, options, nullable);
  } else {
    return neverEver(outputType);
  }
}
