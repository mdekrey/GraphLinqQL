import { Options } from "../Options";
import { GraphQLSchema, GraphQLNamedType, isScalarType } from "graphql";
import { shouldGenerate } from "../shouldGenerate";
import { getTypeName } from "../getTypeName";

export function generateTypeListing(schema: GraphQLSchema, options: Options) {
  return `
public class TypeListing : IGraphQlTypeListing
{
    private readonly IServiceProvider serviceProvider;

    public TypeListing(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Type? Query => ${maybeRenderIntrospectionType(schema.getQueryType())};
    public Type? Mutation => ${maybeRenderIntrospectionType(schema.getMutationType())};
    public Type? Subscription => ${maybeRenderIntrospectionType(schema.getSubscriptionType())};

    public IReadOnlyList<Type> TypeInformation { get; } =
        new []
        {
            ${Object.keys(schema.getTypeMap())
              .map(typeName => schema.getType(typeName))
              .filter(t => shouldGenerate(t!, options) || isScalarType(t!))
              .map(maybeRenderIntrospectionType).join(`,
            `)}
        }.ToImmutableList();
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
