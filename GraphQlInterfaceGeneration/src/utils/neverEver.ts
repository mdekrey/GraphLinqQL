export function neverEver(thing: never): never {
  throw new Error(`Expected ${thing} (typeof ${typeof thing}) to never happen!`);
}
