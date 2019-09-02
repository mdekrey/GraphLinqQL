import { shouldGenerate } from "./shouldGenerate";
import { getStarWarsGraphQlSchemaV4 } from "../resources.test";
import { parseToSchema } from "../parseToSchema";

it("can identify non-system types that should be generated", async function() {
  const schema = await getSampleSchema();
  expect(shouldGenerate(schema.getType("Film")!)).toBeTruthy();
});

it("can identify system types that should not be generated", async function() {
  const schema = await getSampleSchema();
  expect(shouldGenerate(schema.getType("__Schema")!)).toBeFalsy();
});

async function getSampleSchema() {
  const schema = parseToSchema(await getStarWarsGraphQlSchemaV4());
  return schema;
}
