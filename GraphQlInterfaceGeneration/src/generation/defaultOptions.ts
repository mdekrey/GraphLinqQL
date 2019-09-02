import { Options } from "./options";

export const defaultOptions: Options = {
  useNullabilityIndicator: true,
  scalarTypes: {
    Int: { csharpNullable: false, csharpType: "int" },
    Float: { csharpNullable: false, csharpType: "double" },
    String: { csharpNullable: true, csharpType: "string" },
    Boolean: { csharpNullable: false, csharpType: "bool" },
    ID: { csharpNullable: false, csharpType: "GraphQlSchema.GraphQlId" }
  }
};
