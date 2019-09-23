import * as program from "caporal";
import { parseToSchema } from "./parseToSchema";
import { dirname } from "path";
import { readFileSync, writeFileSync, existsSync, mkdirSync } from "fs";
import { defaultOptions } from "./generation/defaultOptions";
import { generateFullFile } from "./generation/generateFullFile";
import { Options } from "./generation/Options";

interface Switches {
  inputFile: string;
  outputFile: string;
  namespace: string;
  csharpLanguage: number;
  using: string[];
  introspection: boolean;
}

program
  .option("-i, --input-file <input>", "Specify input file", program.STRING, null, true)
  .option("-o, --output-file <output>", "Specify output file", program.STRING, null, true)
  .option("-n, --namespace <namespace>", "csharp namespace", program.STRING, null, true)
  .option("--csharp-language", "set csharp language version", program.FLOAT, 8.0, false)
  .option("-u, --using", "additional using statements", program.ARRAY, [], false)
  .option("-_, --introspection", "include introspection properties", program.BOOLEAN, false, false)
  .action(function(_, switches, logger) {
    const inputPath = switches.inputFile;
    const outputPath = switches.outputFile;
    const graphqlSchema = readFileSync(inputPath).toString();
    const parsed = parseToSchema(graphqlSchema);
    const output = generateFullFile(parsed, buildOptions(switches as Switches));
    recursiveMkdir(dirname(outputPath));
    writeFileSync(outputPath, output);
  });

program.parse(process.argv);

function buildOptions(switches: Switches): Options {
  const result: Options = {
    ...defaultOptions,
    namespace: switches.namespace,
    languageVersion: switches.csharpLanguage,
    using: [...defaultOptions.using, ...switches.using],
    introspection: switches.introspection
  };
  return result;
}

function recursiveMkdir(dir: string) {
  if (!existsSync(dir)) {
    recursiveMkdir(dirname(dir));
    mkdirSync(dir);
  }
}
