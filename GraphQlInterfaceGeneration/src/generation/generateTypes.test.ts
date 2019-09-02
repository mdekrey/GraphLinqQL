import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV4 } from "../resources.test";
import { defaultOptions } from "./defaultOptions";
import { generateTypes } from "./generateTypes";

it("can generate v4 types", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  expect(generateTypes(schema, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
