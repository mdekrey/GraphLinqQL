import { Options } from "../Options";

export function useNullabilityIndicator(options: Options) {
  return options.languageVersion >= 8.0;
}
