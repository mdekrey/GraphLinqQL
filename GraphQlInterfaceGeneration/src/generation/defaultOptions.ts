import { Options } from "./Options";

export const defaultOptions: Options = {
  namespace: "GraphQlResolver.Interfaces",
  useNullabilityIndicator: true,
  scalarTypes: {
    Int: { csharpNullable: false, csharpType: "int" },
    Float: { csharpNullable: false, csharpType: "double" },
    String: { csharpNullable: true, csharpType: "string" },
    Boolean: { csharpNullable: false, csharpType: "bool" }
  },
  using: ["GraphQlResolver", "System", "System.Collections.Generic"],
  deserializer: "System.Text.Json.JsonSerializer.Deserialize"
};
