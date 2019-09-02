import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV4 } from "../resources.test";
import { GraphQLObjectType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateType } from "./generateType";

it("can generate simple types", async function() {
  const schema = await getSampleSchema();
  const type = schema.getType("Film") as GraphQLObjectType;
  expect(generateType(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema() {
  const schema = parseToSchema(await getStarWarsGraphQlSchemaV4());
  return schema;
}
