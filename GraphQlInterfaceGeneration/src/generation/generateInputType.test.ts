import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV3 } from "../resources.test";
import { GraphQLInputObjectType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateInputType } from "./generateInputType";

it("can generate input types from v3", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV3);
  const type = schema.getType("ReviewInput") as GraphQLInputObjectType;
  expect(generateInputType(type, defaultOptions)).toMatchSnapshot();
});

it("can generate input types for various default values", async function() {
  const schema = await getSampleSchema(
    async () => `
schema {
  query: Query
}
type Query {
  randomTest(value: RandomInput!
             ): String
}

input RandomInput {
  id: ID = "foo"
  i: Int = 15
  f: Float = 2.5
  b: Boolean = true
  l: [Int!] = [5, 4, 3]
  s: String = "evil\\"\\\\\\"string\\\\"
  o: ColorInput = { red: 255, green: 255, blue: 255 }
  n: ColorInput = null
}

input ColorInput {
  red: Int!
  green: Int!
  blue: Int!
}
`
  );
  const type = schema.getType("RandomInput") as GraphQLInputObjectType;
  expect(generateInputType(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
