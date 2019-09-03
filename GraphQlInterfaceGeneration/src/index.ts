import * as program from "caporal";
import { parseToSchema } from "./parseToSchema";
import { join } from "path";
import { readFileSync, writeFileSync } from "fs";
import { defaultOptions } from "./generation/defaultOptions";
import { generateFullFile } from "./generation/generateFullFile";

program
  .option("-i, --input-file <input>", "Specify input file", program.STRING, null, true)
  .option("-o, --output-file <output>", "Specify output file", program.STRING, null, true)
  .option("-n, --namespace <namespace>", "csharp namespace", program.STRING, "GraphQlSchema", false)
  .action(function(_, options, logger) {
    const inputPath = join(process.cwd(), options.inputFile);
    const outputPath = join(process.cwd(), options.outputFile);
    const graphqlSchema = readFileSync(inputPath).toString();
    const parsed = parseToSchema(graphqlSchema);
    const output = generateFullFile(parsed, {
      ...defaultOptions
    });
    writeFileSync(outputPath, output);
  });

const data = program.parse(process.argv);
