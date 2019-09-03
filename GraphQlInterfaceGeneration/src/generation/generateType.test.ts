import { parseToSchema } from "../parseToSchema";
import { getStarWarsGraphQlSchemaV4 } from "../resources.test";
import { GraphQLObjectType } from "graphql";
import { defaultOptions } from "./defaultOptions";
import { generateType } from "./generateType";

it("can generate v4 types", async function() {
  const schema = await getSampleSchema(getStarWarsGraphQlSchemaV4);
  const type = schema.getType("Person") as GraphQLObjectType;
  expect(generateType(type, defaultOptions)).toMatchSnapshot();
});

it("can generate various default values", async function() {
  const schema = await getSampleSchema(
    async () => `
schema {
  query: Query
}
type Query {
  randomTest(id: ID = "foo",
             i: Int = 15,
             f: Float = 2.5,
             b: Boolean = true,
             l: [Int!] = [5, 4, 3],
             s: String = "evil\\"\\\\\\"string\\\\",
             o: ColorInput = { red: 255, green: 255, blue: 255 },
             n: ColorInput = null
             ): String
}

input ColorInput {
  red: Int!
  green: Int!
  blue: Int!
}
`
  );
  const type = schema.getType("Query") as GraphQLObjectType;
  expect(generateType(type, defaultOptions)).toMatchSnapshot();
});

async function getSampleSchema(getSchema: () => Promise<string>) {
  const schema = parseToSchema(await getSchema());
  return schema;
}
