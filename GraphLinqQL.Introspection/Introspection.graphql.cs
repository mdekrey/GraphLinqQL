using GraphLinqQL;
using GraphLinqQL.Introspection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
#nullable enable
#nullable disable warnings
#pragma warning disable CS0618 // Type or member is obsolete. (Don't check these in the generated file because it only calls itself.)

namespace GraphLinqQL.Introspection.Interfaces
{

    public class TypeResolver : IGraphQlTypeResolver
    {
        public Type Resolve(string name)
        {
            switch (name)
            {
                case "ID":
                    return typeof(string);
                case "Int":
                    return typeof(int?);
                case "Float":
                    return typeof(double?);
                case "String":
                    return typeof(string);
                case "Boolean":
                    return typeof(bool?);

                default:
                    throw new ArgumentException("Unknown type " + name, "name");
            }
        }
    }

    /// <summary>
    /// A GraphQL Schema defines the capabilities of a GraphQL server. It exposes all available types and directives on the server, as well as the entry points for query, mutation, and subscription operations.
    /// </summary>
    public abstract class __Schema : IGraphQlResolvable
    {
        private __Schema() { }

        /// <summary>
        /// A list of all types supported by this server.
        /// </summary>
        public abstract IGraphQlResult<IEnumerable<__Type>> types();

        /// <summary>
        /// The type that query operations will be rooted at.
        /// </summary>
        public abstract IGraphQlResult<__Type> queryType();

        /// <summary>
        /// If this server supports mutation, the type that mutation operations will be rooted at.
        /// </summary>
        public abstract IGraphQlResult<__Type?> mutationType();

        /// <summary>
        /// If this server support subscription, the type that subscription operations will be rooted at.
        /// </summary>
        public abstract IGraphQlResult<__Type?> subscriptionType();

        /// <summary>
        /// A list of all directives supported by this server.
        /// </summary>
        public abstract IGraphQlResult<IEnumerable<__Directive>> directives();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
        {
            switch (name)
            {
                case "__typename": return GraphQlConstantResult.Construct("__Schema");
                case "types": return this.types();
                case "queryType": return this.queryType();
                case "mutationType": return this.mutationType();
                case "subscriptionType": return this.subscriptionType();
                case "directives": return this.directives();
                default: throw new ArgumentException("Unknown property " + name, "name");
            };
        }

        bool IGraphQlResolvable.IsType(string value)
        {
            return value == "__Schema";
        }

        public abstract class GraphQlContract<T> : __Schema, IGraphQlAccepts<T>
        {
            public IGraphQlResultFactory<T> Original { get; set; }
            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    /// <summary>
    /// The fundamental unit of any GraphQL Schema is the type. There are many kinds of types in GraphQL as represented by the `__TypeKind` enum.
    ///
    /// Depending on the kind of a type, certain fields describe information about that type. Scalar types provide no information beyond a name and description, while Enum types provide their values. Object and Interface types provide the fields they describe. Abstract types, Union and Interface, provide the Object types possible at runtime. List and NonNull types compose other types.
    /// </summary>
    public abstract class __Type : IGraphQlResolvable
    {
        private __Type() { }
        public abstract IGraphQlResult<__TypeKind> kind();
        public abstract IGraphQlResult<string?> name();
        public abstract IGraphQlResult<string?> description();
        public abstract IGraphQlResult<IEnumerable<__Field>?> fields(bool? includeDeprecated);
        public abstract IGraphQlResult<IEnumerable<__Type>?> interfaces();
        public abstract IGraphQlResult<IEnumerable<__Type>?> possibleTypes();
        public abstract IGraphQlResult<IEnumerable<__EnumValue>?> enumValues(bool? includeDeprecated);
        public abstract IGraphQlResult<IEnumerable<__InputValue>?> inputFields();
        public abstract IGraphQlResult<__Type?> ofType();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
        {
            switch (name)
            {
                case "__typename": return GraphQlConstantResult.Construct("__Type");
                case "kind": return this.kind();
                case "name": return this.name();
                case "description": return this.description();
                case "fields": return this.fields(
                    includeDeprecated: (parameters.HasParameter("includeDeprecated") ? parameters.GetParameter<bool?>("includeDeprecated") : null));
                case "interfaces": return this.interfaces();
                case "possibleTypes": return this.possibleTypes();
                case "enumValues": return this.enumValues(
                    includeDeprecated: (parameters.HasParameter("includeDeprecated") ? parameters.GetParameter<bool?>("includeDeprecated") : null));
                case "inputFields": return this.inputFields();
                case "ofType": return this.ofType();
                default: throw new ArgumentException("Unknown property " + name, "name");
            };
        }

        bool IGraphQlResolvable.IsType(string value)
        {
            return value == "__Type";
        }

        public abstract class GraphQlContract<T> : __Type, IGraphQlAccepts<T>
        {
            public IGraphQlResultFactory<T> Original { get; set; }
            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    /// <summary>
    /// An enum describing what kind of type a given `__Type` is.
    /// </summary>
    public enum __TypeKind
    {

        /// <summary>
        /// Indicates this type is a scalar.
        /// </summary>
        SCALAR,

        /// <summary>
        /// Indicates this type is an object. `fields` and `interfaces` are valid fields.
        /// </summary>
        OBJECT,

        /// <summary>
        /// Indicates this type is an interface. `fields` and `possibleTypes` are valid fields.
        /// </summary>
        INTERFACE,

        /// <summary>
        /// Indicates this type is a union. `possibleTypes` is a valid field.
        /// </summary>
        UNION,

        /// <summary>
        /// Indicates this type is an enum. `enumValues` is a valid field.
        /// </summary>
        ENUM,

        /// <summary>
        /// Indicates this type is an input object. `inputFields` is a valid field.
        /// </summary>
        INPUT_OBJECT,

        /// <summary>
        /// Indicates this type is a list. `ofType` is a valid field.
        /// </summary>
        LIST,

        /// <summary>
        /// Indicates this type is a non-null. `ofType` is a valid field.
        /// </summary>
        NON_NULL
    }


    /// <summary>
    /// Object and Interface types are described by a list of Fields, each of which has a name, potentially a list of arguments, and a return type.
    /// </summary>
    public abstract class __Field : IGraphQlResolvable
    {
        private __Field() { }
        public abstract IGraphQlResult<string> name();
        public abstract IGraphQlResult<string?> description();
        public abstract IGraphQlResult<IEnumerable<__InputValue>> args();
        public abstract IGraphQlResult<__Type> type();
        public abstract IGraphQlResult<bool> isDeprecated();
        public abstract IGraphQlResult<string?> deprecationReason();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
        {
            switch (name)
            {
                case "__typename": return GraphQlConstantResult.Construct("__Field");
                case "name": return this.name();
                case "description": return this.description();
                case "args": return this.args();
                case "type": return this.type();
                case "isDeprecated": return this.isDeprecated();
                case "deprecationReason": return this.deprecationReason();
                default: throw new ArgumentException("Unknown property " + name, "name");
            };
        }

        bool IGraphQlResolvable.IsType(string value)
        {
            return value == "__Field";
        }

        public abstract class GraphQlContract<T> : __Field, IGraphQlAccepts<T>
        {
            public IGraphQlResultFactory<T> Original { get; set; }
            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    /// <summary>
    /// Arguments provided to Fields or Directives and the input fields of an InputObject are represented as Input Values which describe their type and optionally a default value.
    /// </summary>
    public abstract class __InputValue : IGraphQlResolvable
    {
        private __InputValue() { }
        public abstract IGraphQlResult<string> name();
        public abstract IGraphQlResult<string?> description();
        public abstract IGraphQlResult<__Type> type();

        /// <summary>
        /// A GraphQL-formatted string representing the default value for this input value.
        /// </summary>
        public abstract IGraphQlResult<string?> defaultValue();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
        {
            switch (name)
            {
                case "__typename": return GraphQlConstantResult.Construct("__InputValue");
                case "name": return this.name();
                case "description": return this.description();
                case "type": return this.type();
                case "defaultValue": return this.defaultValue();
                default: throw new ArgumentException("Unknown property " + name, "name");
            };
        }

        bool IGraphQlResolvable.IsType(string value)
        {
            return value == "__InputValue";
        }

        public abstract class GraphQlContract<T> : __InputValue, IGraphQlAccepts<T>
        {
            public IGraphQlResultFactory<T> Original { get; set; }
            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    /// <summary>
    /// One possible value for a given Enum. Enum values are unique values, not a placeholder for a string or numeric value. However an Enum value is returned in a JSON response as a string.
    /// </summary>
    public abstract class __EnumValue : IGraphQlResolvable
    {
        private __EnumValue() { }
        public abstract IGraphQlResult<string> name();
        public abstract IGraphQlResult<string?> description();
        public abstract IGraphQlResult<bool> isDeprecated();
        public abstract IGraphQlResult<string?> deprecationReason();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
        {
            switch (name)
            {
                case "__typename": return GraphQlConstantResult.Construct("__EnumValue");
                case "name": return this.name();
                case "description": return this.description();
                case "isDeprecated": return this.isDeprecated();
                case "deprecationReason": return this.deprecationReason();
                default: throw new ArgumentException("Unknown property " + name, "name");
            };
        }

        bool IGraphQlResolvable.IsType(string value)
        {
            return value == "__EnumValue";
        }

        public abstract class GraphQlContract<T> : __EnumValue, IGraphQlAccepts<T>
        {
            public IGraphQlResultFactory<T> Original { get; set; }
            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    /// <summary>
    /// A Directive provides a way to describe alternate runtime execution and type validation behavior in a GraphQL document.
    ///
    /// In some cases, you need to provide options to alter GraphQL's execution behavior in ways field arguments will not suffice, such as conditionally including or skipping a field. Directives provide this by describing additional information to the executor.
    /// </summary>
    public abstract class __Directive : IGraphQlResolvable
    {
        private __Directive() { }
        public abstract IGraphQlResult<string> name();
        public abstract IGraphQlResult<string?> description();
        public abstract IGraphQlResult<IEnumerable<__DirectiveLocation>> locations();
        public abstract IGraphQlResult<IEnumerable<__InputValue>> args();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters)
        {
            switch (name)
            {
                case "__typename": return GraphQlConstantResult.Construct("__Directive");
                case "name": return this.name();
                case "description": return this.description();
                case "locations": return this.locations();
                case "args": return this.args();
                default: throw new ArgumentException("Unknown property " + name, "name");
            };
        }

        bool IGraphQlResolvable.IsType(string value)
        {
            return value == "__Directive";
        }

        public abstract class GraphQlContract<T> : __Directive, IGraphQlAccepts<T>
        {
            public IGraphQlResultFactory<T> Original { get; set; }
            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    /// <summary>
    /// A Directive can be adjacent to many parts of the GraphQL language, a __DirectiveLocation describes one such possible adjacencies.
    /// </summary>
    public enum __DirectiveLocation
    {

        /// <summary>
        /// Location adjacent to a query operation.
        /// </summary>
        QUERY,

        /// <summary>
        /// Location adjacent to a mutation operation.
        /// </summary>
        MUTATION,

        /// <summary>
        /// Location adjacent to a subscription operation.
        /// </summary>
        SUBSCRIPTION,

        /// <summary>
        /// Location adjacent to a field.
        /// </summary>
        FIELD,

        /// <summary>
        /// Location adjacent to a fragment definition.
        /// </summary>
        FRAGMENT_DEFINITION,

        /// <summary>
        /// Location adjacent to a fragment spread.
        /// </summary>
        FRAGMENT_SPREAD,

        /// <summary>
        /// Location adjacent to an inline fragment.
        /// </summary>
        INLINE_FRAGMENT,

        /// <summary>
        /// Location adjacent to a variable definition.
        /// </summary>
        VARIABLE_DEFINITION,

        /// <summary>
        /// Location adjacent to a schema definition.
        /// </summary>
        SCHEMA,

        /// <summary>
        /// Location adjacent to a scalar definition.
        /// </summary>
        SCALAR,

        /// <summary>
        /// Location adjacent to an object type definition.
        /// </summary>
        OBJECT,

        /// <summary>
        /// Location adjacent to a field definition.
        /// </summary>
        FIELD_DEFINITION,

        /// <summary>
        /// Location adjacent to an argument definition.
        /// </summary>
        ARGUMENT_DEFINITION,

        /// <summary>
        /// Location adjacent to an interface definition.
        /// </summary>
        INTERFACE,

        /// <summary>
        /// Location adjacent to a union definition.
        /// </summary>
        UNION,

        /// <summary>
        /// Location adjacent to an enum definition.
        /// </summary>
        ENUM,

        /// <summary>
        /// Location adjacent to an enum value definition.
        /// </summary>
        ENUM_VALUE,

        /// <summary>
        /// Location adjacent to an input object type definition.
        /// </summary>
        INPUT_OBJECT,

        /// <summary>
        /// Location adjacent to an input object field definition.
        /// </summary>
        INPUT_FIELD_DEFINITION
    }


    namespace Introspection
    {

        public class TypeListing : IGraphQlTypeListing
        {
            private static readonly ImmutableDictionary<string, Type> types = new Dictionary<string, Type>
            {
                { "__Schema", typeof(Introspection.__Schema) },
                { "__Type", typeof(Introspection.__Type) },
                { "__TypeKind", typeof(Introspection.__TypeKind) },
                { "String", typeof(Introspection.String) },
                { "Boolean", typeof(Introspection.Boolean) },
                { "Int", typeof(Introspection.Int) },
                { "Float", typeof(Introspection.Float) },
                { "__Field", typeof(Introspection.__Field) },
                { "__InputValue", typeof(Introspection.__InputValue) },
                { "__EnumValue", typeof(Introspection.__EnumValue) },
                { "__Directive", typeof(Introspection.__Directive) },
                { "__DirectiveLocation", typeof(Introspection.__DirectiveLocation) },
                { "ID", typeof(Introspection.ID) }
            }.ToImmutableDictionary();
            private static readonly IReadOnlyList<DirectiveInformation> directives = new DirectiveInformation[] {

                    new DirectiveInformation(name: "skip",
                                             locations: new[] { DirectiveLocation.Field, DirectiveLocation.FragmentSpread, DirectiveLocation.InlineFragment },
                                             args: new GraphQlInputFieldInformation[] {
                                new GraphQlInputFieldInformation(name: "if", type: typeof(NonNullTypeInformation<Boolean>), description: @"Skipped when true.")
                            },
                                             description: @"Directs the executor to skip this field or fragment when the `if` argument is true."),

                    new DirectiveInformation(name: "include",
                                             locations: new[] { DirectiveLocation.Field, DirectiveLocation.FragmentSpread, DirectiveLocation.InlineFragment },
                                             args: new GraphQlInputFieldInformation[] {
                                new GraphQlInputFieldInformation(name: "if", type: typeof(NonNullTypeInformation<Boolean>), description: @"Included when true.")
                            },
                                             description: @"Directs the executor to include this field or fragment only when the `if` argument is true."),

                    new DirectiveInformation(name: "deprecated",
                                             locations: new[] { DirectiveLocation.FieldDefinition, DirectiveLocation.EnumValue },
                                             args: new GraphQlInputFieldInformation[] {
                                new GraphQlInputFieldInformation(name: "reason", type: typeof(String), description: @"Explains why this element was deprecated, usually also including a suggestion for how to access supported similar data. Formatted using the Markdown syntax (as specified by [CommonMark](https://commonmark.org/).", defaultValue: @"No longer supported")
                            },
                                             description: @"Marks an element of a GraphQL schema as no longer supported.")
            };

            public Type Query { get { return null; } }
            public Type? Mutation { get { return null; } }
            public Type? Subscription { get { return null; } }

            public IEnumerable<Type> TypeInformation { get { return types.Values; } }
            public IEnumerable<DirectiveInformation> DirectiveInformation { get { return directives; } }

            public Type? Type(string name)
            {
                Type type;
                return types.TryGetValue(name, out type) ? type : null;
            }
        }
        public class __Schema : IGraphQlTypeInformation
        {
            public string Name { get { return "__Schema"; } }
            public string? Description { get { return @"A GraphQL Schema defines the capabilities of a GraphQL server. It exposes all available types and directives on the server, as well as the entry points for query, mutation, and subscription operations."; } }
            public TypeKind Kind { get { return TypeKind.Object; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return new Type[] {

            }.ToImmutableList(); } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<ListTypeInformation<NonNullTypeInformation<__Type>>>),
                    name: "types",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: @"A list of all types supported by this server.",
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<__Type>),
                    name: "queryType",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: @"The type that query operations will be rooted at.",
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(__Type),
                    name: "mutationType",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: @"If this server supports mutation, the type that mutation operations will be rooted at.",
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(__Type),
                    name: "subscriptionType",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: @"If this server support subscription, the type that subscription operations will be rooted at.",
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<ListTypeInformation<NonNullTypeInformation<__Directive>>>),
                    name: "directives",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: @"A list of all directives supported by this server.",
                    isDeprecated: false,
                    deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
            }
        }

        public class __Type : IGraphQlTypeInformation
        {
            public string Name { get { return "__Type"; } }
            public string? Description { get { return @"The fundamental unit of any GraphQL Schema is the type. There are many kinds of types in GraphQL as represented by the `__TypeKind` enum.

        Depending on the kind of a type, certain fields describe information about that type. Scalar types provide no information beyond a name and description, while Enum types provide their values. Object and Interface types provide the fields they describe. Abstract types, Union and Interface, provide the Object types possible at runtime. List and NonNull types compose other types."; } }
            public TypeKind Kind { get { return TypeKind.Object; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return new Type[] {

            }.ToImmutableList(); } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<__TypeKind>),
                    name: "kind",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "name",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "description",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(ListTypeInformation<NonNullTypeInformation<__Field>>),
                    name: "fields",
                    args: new GraphQlInputFieldInformation[] {
                        new GraphQlInputFieldInformation(name: "includeDeprecated", type: typeof(Boolean), description: null, defaultValue: @"false")
                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(ListTypeInformation<NonNullTypeInformation<__Type>>),
                    name: "interfaces",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(ListTypeInformation<NonNullTypeInformation<__Type>>),
                    name: "possibleTypes",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(ListTypeInformation<NonNullTypeInformation<__EnumValue>>),
                    name: "enumValues",
                    args: new GraphQlInputFieldInformation[] {
                        new GraphQlInputFieldInformation(name: "includeDeprecated", type: typeof(Boolean), description: null, defaultValue: @"false")
                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(ListTypeInformation<NonNullTypeInformation<__InputValue>>),
                    name: "inputFields",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(__Type),
                    name: "ofType",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
            }
        }

        public class __TypeKind : IGraphQlTypeInformation
        {
            public string Name { get { return "__TypeKind"; } }
            public string? Description { get { return @"An enum describing what kind of type a given `__Type` is."; } }
            public TypeKind Kind { get { return TypeKind.Enum; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            private static readonly IReadOnlyList<GraphQlEnumValueInformation> enumValues = new GraphQlEnumValueInformation[] {
                new GraphQlEnumValueInformation(name: "SCALAR", description: @"Indicates this type is a scalar.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "OBJECT", description: @"Indicates this type is an object. `fields` and `interfaces` are valid fields.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "INTERFACE", description: @"Indicates this type is an interface. `fields` and `possibleTypes` are valid fields.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "UNION", description: @"Indicates this type is a union. `possibleTypes` is a valid field.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "ENUM", description: @"Indicates this type is an enum. `enumValues` is a valid field.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "INPUT_OBJECT", description: @"Indicates this type is an input object. `inputFields` is a valid field.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "LIST", description: @"Indicates this type is a list. `ofType` is a valid field.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "NON_NULL", description: @"Indicates this type is a non-null. `ofType` is a valid field.", isDeprecated: false, deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return enumValues.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

        public class String : IGraphQlTypeInformation
        {
            public string Name { get { return "String"; } }
            public string? Description { get { return @"The `String` scalar type represents textual data, represented as UTF-8 character sequences. The String type is most often used by GraphQL to represent free-form human-readable text."; } }
            public TypeKind Kind { get { return TypeKind.Scalar; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

        public class Boolean : IGraphQlTypeInformation
        {
            public string Name { get { return "Boolean"; } }
            public string? Description { get { return @"The `Boolean` scalar type represents `true` or `false`."; } }
            public TypeKind Kind { get { return TypeKind.Scalar; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

        public class __Field : IGraphQlTypeInformation
        {
            public string Name { get { return "__Field"; } }
            public string? Description { get { return @"Object and Interface types are described by a list of Fields, each of which has a name, potentially a list of arguments, and a return type."; } }
            public TypeKind Kind { get { return TypeKind.Object; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return new Type[] {

            }.ToImmutableList(); } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<String>),
                    name: "name",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "description",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<ListTypeInformation<NonNullTypeInformation<__InputValue>>>),
                    name: "args",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<__Type>),
                    name: "type",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<Boolean>),
                    name: "isDeprecated",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "deprecationReason",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
            }
        }

        public class __InputValue : IGraphQlTypeInformation
        {
            public string Name { get { return "__InputValue"; } }
            public string? Description { get { return @"Arguments provided to Fields or Directives and the input fields of an InputObject are represented as Input Values which describe their type and optionally a default value."; } }
            public TypeKind Kind { get { return TypeKind.Object; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return new Type[] {

            }.ToImmutableList(); } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<String>),
                    name: "name",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "description",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<__Type>),
                    name: "type",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "defaultValue",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: @"A GraphQL-formatted string representing the default value for this input value.",
                    isDeprecated: false,
                    deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
            }
        }

        public class __EnumValue : IGraphQlTypeInformation
        {
            public string Name { get { return "__EnumValue"; } }
            public string? Description { get { return @"One possible value for a given Enum. Enum values are unique values, not a placeholder for a string or numeric value. However an Enum value is returned in a JSON response as a string."; } }
            public TypeKind Kind { get { return TypeKind.Object; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return new Type[] {

            }.ToImmutableList(); } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<String>),
                    name: "name",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "description",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<Boolean>),
                    name: "isDeprecated",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "deprecationReason",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
            }
        }

        public class __Directive : IGraphQlTypeInformation
        {
            public string Name { get { return "__Directive"; } }
            public string? Description { get { return @"A Directive provides a way to describe alternate runtime execution and type validation behavior in a GraphQL document.

        In some cases, you need to provide options to alter GraphQL's execution behavior in ways field arguments will not suffice, such as conditionally including or skipping a field. Directives provide this by describing additional information to the executor."; } }
            public TypeKind Kind { get { return TypeKind.Object; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return new Type[] {

            }.ToImmutableList(); } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            private static readonly IReadOnlyList<GraphQlFieldInformation> fields = new GraphQlFieldInformation[] {
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<String>),
                    name: "name",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(String),
                    name: "description",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<ListTypeInformation<NonNullTypeInformation<__DirectiveLocation>>>),
                    name: "locations",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null),
                new GraphQlFieldInformation(
                    type: typeof(NonNullTypeInformation<ListTypeInformation<NonNullTypeInformation<__InputValue>>>),
                    name: "args",
                    args: new GraphQlInputFieldInformation[] {

                    },
                    description: null,
                    isDeprecated: false,
                    deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return fields.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));
            }
        }

        public class __DirectiveLocation : IGraphQlTypeInformation
        {
            public string Name { get { return "__DirectiveLocation"; } }
            public string? Description { get { return @"A Directive can be adjacent to many parts of the GraphQL language, a __DirectiveLocation describes one such possible adjacencies."; } }
            public TypeKind Kind { get { return TypeKind.Enum; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            private static readonly IReadOnlyList<GraphQlEnumValueInformation> enumValues = new GraphQlEnumValueInformation[] {
                new GraphQlEnumValueInformation(name: "QUERY", description: @"Location adjacent to a query operation.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "MUTATION", description: @"Location adjacent to a mutation operation.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "SUBSCRIPTION", description: @"Location adjacent to a subscription operation.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "FIELD", description: @"Location adjacent to a field.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "FRAGMENT_DEFINITION", description: @"Location adjacent to a fragment definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "FRAGMENT_SPREAD", description: @"Location adjacent to a fragment spread.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "INLINE_FRAGMENT", description: @"Location adjacent to an inline fragment.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "VARIABLE_DEFINITION", description: @"Location adjacent to a variable definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "SCHEMA", description: @"Location adjacent to a schema definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "SCALAR", description: @"Location adjacent to a scalar definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "OBJECT", description: @"Location adjacent to an object type definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "FIELD_DEFINITION", description: @"Location adjacent to a field definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "ARGUMENT_DEFINITION", description: @"Location adjacent to an argument definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "INTERFACE", description: @"Location adjacent to an interface definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "UNION", description: @"Location adjacent to a union definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "ENUM", description: @"Location adjacent to an enum definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "ENUM_VALUE", description: @"Location adjacent to an enum value definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "INPUT_OBJECT", description: @"Location adjacent to an input object type definition.", isDeprecated: false, deprecationReason: null),
                new GraphQlEnumValueInformation(name: "INPUT_FIELD_DEFINITION", description: @"Location adjacent to an input object field definition.", isDeprecated: false, deprecationReason: null)
            }.ToImmutableList();
            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return enumValues.Where(v => !v.IsDeprecated || (includeDeprecated ?? false));;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

        public class ID : IGraphQlTypeInformation
        {
            public string Name { get { return "ID"; } }
            public string? Description { get { return @"The `ID` scalar type represents a unique identifier, often used to refetch an object or as key for a cache. The ID type appears in a JSON response as a String; however, it is not intended to be human-readable. When expected as an input type, any string (such as `""4""`) or integer (such as `4`) input value will be accepted as an ID."; } }
            public TypeKind Kind { get { return TypeKind.Scalar; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

        public class Float : IGraphQlTypeInformation
        {
            public string Name { get { return "Float"; } }
            public string? Description { get { return @"The `Float` scalar type represents signed double-precision fractional values as specified by [IEEE 754](https://en.wikipedia.org/wiki/IEEE_floating_point)."; } }
            public TypeKind Kind { get { return TypeKind.Scalar; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

        public class Int : IGraphQlTypeInformation
        {
            public string Name { get { return "Int"; } }
            public string? Description { get { return @"The `Int` scalar type represents non-fractional signed whole numeric values. Int can represent values between -(2^31) and 2^31 - 1."; } }
            public TypeKind Kind { get { return TypeKind.Scalar; } }
            public Type? OfType { get { return null; } }
            public IReadOnlyList<Type>? Interfaces { get { return null; } }
            public IReadOnlyList<Type>? PossibleTypes { get { return null; } }

            public IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get { return null; } }


            public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated)
            {
                return null;
            }

            public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated)
            {
                return null;
            }
        }

    }

}
