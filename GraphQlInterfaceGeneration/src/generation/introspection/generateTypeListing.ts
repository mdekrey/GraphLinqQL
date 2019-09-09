import { Options } from "../Options";
import { GraphQLSchema, GraphQLNamedType, isScalarType } from "graphql";
import { shouldGenerate } from "../shouldGenerate";
import { getTypeName } from "../getTypeName";
import { generateDirectiveInfo } from "./generateDirectiveInfo";

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

    private readonly IServiceProvider serviceProvider;

    public TypeListing(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Type? Query => ${maybeRenderIntrospectionType(schema.getQueryType())};
    public Type? Mutation => ${maybeRenderIntrospectionType(schema.getMutationType())};
    public Type? Subscription => ${maybeRenderIntrospectionType(schema.getSubscriptionType())};

    public IEnumerable<Type> TypeInformation => types.Values;
    public IEnumerable<DirectiveInformation> DirectiveInformation => directives;

    public Type? Type(string name) => types.TryGetValue(name, out var type) ? type : null;
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
