import { readFile } from "fs";

export function getStarWarsGraphQlSchemaV3() {
  return readFilePromise(__dirname + "/sw-schema.3.graphql");
}

export function getStarWarsGraphQlSchemaV4() {
  return readFilePromise(__dirname + "/sw-schema.4.graphql");
}

function readFilePromise(path: string) {
  return new Promise<string>((resolve, reject) =>
    readFile(path, (err, data) => (err ? reject(err) : resolve(data.toString())))
  );
}
