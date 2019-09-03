import {
  isNonNullType,
  isScalarType,
  isEnumType,
  isUnionType,
  isInterfaceType,
  isListType,
  GraphQLInputType,
  isInputObjectType
} from "graphql";
import { Options } from "./Options";
import { neverEver } from "../utils/neverEver";
import { getOutputTypeName } from "./getOutputTypeName";
import { getScalarTypeName } from "./getScalarTypeName";
import { getListTypeName } from "./getListTypeName";
import { getEnumTypeName } from "./getEnumTypeName";
import { getInputObjectTypeName } from "./getInputObjectTypeName";

export function getInputTypeName(inputType: GraphQLInputType, options: Options, nullable: boolean = true): string {
  // This is a bit awkward, as C# assumes not-nullable and GraphQL assumes nullable
  if (isNonNullType(inputType)) {
    if (!nullable) {
      throw new Error("Not nullable in an already not-nullable context! " + inputType.toString());
    }
    return getInputTypeName(inputType.ofType, options, false);
  }
  if (isScalarType(inputType)) {
    return getScalarTypeName(inputType, options, nullable);
  } else if (isListType(inputType)) {
    return getListTypeName(inputType, options, nullable, getInputTypeName);
  } else if (isEnumType(inputType)) {
    return getEnumTypeName(inputType, options, nullable);
  } else if (isInputObjectType(inputType)) {
    return getInputObjectTypeName(inputType, options, nullable);
  } else {
    return neverEver(inputType);
  }
}
