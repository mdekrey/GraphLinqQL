export type ScalarTypeMapping = Record<string, { csharpNullable: boolean; csharpType: string }>;

export interface Options {
  useNullabilityIndicator: boolean;
  scalarTypes: ScalarTypeMapping;
  namespace: string;
  using: string[];
  deserializer: string;
  introspection: boolean;
}
