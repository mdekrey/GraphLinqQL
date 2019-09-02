import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV3 } from "../resources.test";
import { GraphQLInputObjectType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateInputType } from "./generateInputType";

it("can generate input types from v3", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV3);
  const type = schema.getType("ReviewInput") as GraphQLInputObjectType;
  expect(generateInputType(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
