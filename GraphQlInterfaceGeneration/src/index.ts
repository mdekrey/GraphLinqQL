import * as program from "caporal";
import { parseToSchema } from "./parseToSchema";
import { join } from "path";
import { readFileSync, writeFileSync } from "fs";
import { defaultOptions } from "./generation/defaultOptions";
import { generateFullFile } from "./generation/generateFullFile";
import { Options } from "./generation/Options";

interface Switches {
  inputFile: string;
  outputFile: string;
  namespace: string;
  noNullability: boolean;
  using: string[];
}

program
  .option("-i, --input-file <input>", "Specify input file", program.STRING, null, true)
  .option("-o, --output-file <output>", "Specify output file", program.STRING, null, true)
  .option("-n, --namespace <namespace>", "csharp namespace", program.STRING, "GraphQlResolver.Interfaces", false)
  .option("--no-nullability", "disable nullability flags (which requires C# 8)", program.BOOLEAN, false, false)
  .option("-u, --using", "additional using statements", program.ARRAY, [], false)
  .action(function(_, switches, logger) {
    const inputPath = switches.inputFile;
    const outputPath = switches.outputFile;
    const graphqlSchema = readFileSync(inputPath).toString();
    const parsed = parseToSchema(graphqlSchema);
    const output = generateFullFile(parsed, buildOptions(switches as Switches));
    writeFileSync(outputPath, output);
  });

program.parse(process.argv);

function buildOptions(switches: Switches): Options {
  const result: Options = {
    ...defaultOptions,
    namespace: switches.namespace,
    useNullabilityIndicator: !switches.noNullability,
    using: [...defaultOptions.using, ...switches.using]
  };
  return result;
}
