import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV4, getStarWarsGraphQlSchemaV3 } from "../resources.test";
import { GraphQLInterfaceType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateInterface } from "./generateInterface";

it("can generate simple interfaces", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  const type = schema.getType("Node") as GraphQLInterfaceType;
  expect(generateInterface(type, defaultOptions)).toMatchSnapshot();
});

it("can generate simple interfaces from v3", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV3);
  const type = schema.getType("Character") as GraphQLInterfaceType;
  expect(generateInterface(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
