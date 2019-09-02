import { parse, buildASTSchema, GraphQLSchema } from "graphql";

export function parseToSchema(graphQlSchema: string): GraphQLSchema {
  const astDocument = parse(graphQlSchema);

  const backcompatOptions = { commentDescriptions: true };
  const schema = buildASTSchema(astDocument, backcompatOptions);

  return schema;
}
