import { GraphQLDirective, DirectiveLocationEnum } from "graphql";
import { Options } from "../Options";
import { multilineString } from "./multilineString";
import { toInputObjectInfoArray } from "./toInputObjectInfoArray";

export function generateDirectiveInfo(directive: GraphQLDirective, options: Options) {
  return `
    new DirectiveInformation(name: "${directive.name}",
                             locations: new[] { ${directive.locations.map(toDirectiveLocation).join(", ")} },
                             args: ${toInputObjectInfoArray(directive.args, options)},
                             description: ${multilineString(directive.description)})`;
}

function toDirectiveLocation(d: DirectiveLocationEnum) {
  return `DirectiveLocation.${d.toLocaleLowerCase().replace(/(^|_)./g, s => s.replace("_", "").toUpperCase())}`;
}
