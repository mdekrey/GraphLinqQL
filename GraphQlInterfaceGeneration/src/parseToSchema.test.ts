import { parseToSchema } from "./parseToSchema";
import { getStarWarsGraphQlSchemaV3, getStarWarsGraphQlSchemaV4 } from "./resources.test";

it("can parse graphql v3 schemas", async function() {
  const schema = parseToSchema(await getStarWarsGraphQlSchemaV3());
  expect(schema).not.toBeNull();
  expect(Object.keys(schema.getTypeMap())).toMatchSnapshot();
});

it("can parse graphql v4 schemas", async function() {
  const schema = parseToSchema(await getStarWarsGraphQlSchemaV4());
  expect(schema).not.toBeNull();
  expect(schema.getType("Film")!.description).toEqual("A single film.");
  expect(Object.keys(schema.getTypeMap())).toMatchSnapshot();
});
