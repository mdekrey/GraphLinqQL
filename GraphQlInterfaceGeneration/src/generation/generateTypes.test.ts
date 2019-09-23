import { parseToSchema } from "../parseToSchema";
import {
  getStarWarsGraphQlSchemaV4,
  getStarWarsGraphQlSchemaV3,
  getStarWarsGraphQlSchemaWithDeprecationsV3
} from "../resources.test";
import { defaultOptions } from "./defaultOptions";
import { generateTypes } from "./generateTypes";

it("can generate v3 types", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV3);
  expect(generateTypes(schema, defaultOptions)).toMatchSnapshot();
});

it("can generate v3 types with deprecations", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaWithDeprecationsV3);
  expect(generateTypes(schema, defaultOptions)).toMatchSnapshot();
});

it("can generate v4 types", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  expect(generateTypes(schema, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
