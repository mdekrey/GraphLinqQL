import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV3 } from "../resources.test";
import { defaultOptions } from "./defaultOptions";
import { generateTypeResolver } from "./generateTypeResolver";

it("can generate v3 types", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV3);
  expect(generateTypeResolver(schema, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
