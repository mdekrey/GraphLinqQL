import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV4 } from "../resources.test";
import { GraphQLObjectType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateType } from "./generateType";

it("can generate v4 types", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  const type = schema.getType("Person") as GraphQLObjectType;
  expect(generateType(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
