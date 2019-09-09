export function multilineString(string: string | null | undefined) {
  return string === null || string === undefined ? null : `@"${string.replace(/"/g, '""')}"`;
}
