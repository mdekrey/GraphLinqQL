import { Options } from "./Options";

export const defaultOptions: Options = {
  namespace: "GraphQlInterfaces",
  useNullabilityIndicator: true,
  scalarTypes: {
    Int: { csharpNullable: false, csharpType: "int" },
    Float: { csharpNullable: false, csharpType: "double" },
    String: { csharpNullable: true, csharpType: "string" },
    Boolean: { csharpNullable: false, csharpType: "bool" },
    ID: { csharpNullable: true, csharpType: "GraphQlResolver.CommonTypes.GraphQlId" }
  },
  using: ["GraphQlSchema", "System", "System.Collections.Generic"]
};
