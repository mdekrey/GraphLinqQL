import { Options } from "../Options";
import { GraphQLSchema, GraphQLNamedType, isScalarType } from "graphql";
import { shouldGenerate } from "../shouldGenerate";
import { getTypeName } from "../getTypeName";
import { generateDirectiveInfo } from "./generateDirectiveInfo";
import { useNullabilityIndicator } from "../options/useNullabilityIndicator";

export function generateTypeListing(schema: GraphQLSchema, options: Options) {
  return `
public class TypeListing : IGraphQlTypeListing
{
    private static readonly ImmutableDictionary<string, Type> types = new Dictionary<string, Type>
    {
        ${Object.keys(schema.getTypeMap())
          .map(typeName => schema.getType(typeName))
          .filter(t => shouldGenerate(t!, options) || isScalarType(t!))
          .map(t => `{ "${t!.name}", ${maybeRenderIntrospectionType(t)} }`).join(`,
        `)}
    }.ToImmutableDictionary();
    private static readonly IReadOnlyList<DirectiveInformation> directives = new DirectiveInformation[] {
      ${schema
        .getDirectives()
        .map(directive => generateDirectiveInfo(directive, options))
        .join(",\n")
        .split("\n").join(`
        `)}
    };

    public Type Query { get { return ${maybeRenderIntrospectionType(schema.getQueryType())}; } }
    public Type${useNullabilityIndicator(options) ? "?" : ""} Mutation { get { return ${maybeRenderIntrospectionType(
    schema.getMutationType()
  )}; } }
    public Type${
      useNullabilityIndicator(options) ? "?" : ""
    } Subscription { get { return ${maybeRenderIntrospectionType(schema.getSubscriptionType())}; } }

    public IEnumerable<Type> TypeInformation { get { return types.Values; } }
    public IEnumerable<DirectiveInformation> DirectiveInformation { get { return directives; } }

    public Type${useNullabilityIndicator(options) ? "?" : ""} Type(string name)
    {
        Type type;
        return types.TryGetValue(name, out type) ? type : null;
    }
}`;
  function getTypeForIntrospection(t: GraphQLNamedType) {
    return getTypeName(t.name, options);
  }
  function maybeRenderIntrospectionType(t: GraphQLNamedType | null | undefined) {
    if (!t) {
      return "null";
    }
    return `typeof(Introspection.${getTypeForIntrospection(t)})`;
  }
}
