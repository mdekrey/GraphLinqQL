import { shouldGenerate } from "./shouldGenerate";
import { getStarWarsGraphQlSchemaV4 } from "../resources.test";
import { parseToSchema } from "../parseToSchema";
import { defaultOptions } from "./defaultOptions";

it("can identify non-system types that should be generated", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  expect(shouldGenerate(schema.getType("Film")!, defaultOptions)).toBeTruthy();
});

it("can identify system types that should not be generated", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  expect(shouldGenerate(schema.getType("__Schema")!, defaultOptions)).toBeFalsy();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
