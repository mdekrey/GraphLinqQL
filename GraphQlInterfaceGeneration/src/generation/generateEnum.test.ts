import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV3 } from "../resources.test";
import { GraphQLEnumType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateEnum } from "./generateEnum";

it("can generate v3 enum", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV3);
  const type = schema.getType("Episode") as GraphQLEnumType;
  expect(generateEnum(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
