namespace GraphLinqQL.Introspection
{
    public enum DirectiveLocation
    {
        /// <summary>
        /// Location adjacent to a query operation.
        /// </summary>
        Query,

        /// <summary>
        /// Location adjacent to a mutation operation.
        /// </summary>
        Mutation,

        /// <summary>
        /// Location adjacent to a subscription operation.
        /// </summary>
        Subscription,

        /// <summary>
        /// Location adjacent to a field.
        /// </summary>
        Field,

        /// <summary>
        /// Location adjacent to a fragment definition.
        /// </summary>
        FragmentDefinition,

        /// <summary>
        /// Location adjacent to a fragment spread.
        /// </summary>
        FragmentSpread,

        /// <summary>
        /// Location adjacent to an inline fragment.
        /// </summary>
        InlineFragment,

        /// <summary>
        /// Location adjacent to a variable definition.
        /// </summary>
        VariableDefinition,

        /// <summary>
        /// Location adjacent to a schema definition.
        /// </summary>
        Schema,

        /// <summary>
        /// Location adjacent to a scalar definition.
        /// </summary>
        Scalar,

#pragma warning disable CA1720 // Identifier contains type name - generated from graphql
        /// <summary>
        /// Location adjacent to an object type definition.
        /// </summary>
        Object,
#pragma warning restore CA1720 // Identifier contains type name

        /// <summary>
        /// Location adjacent to a field definition.
        /// </summary>
        FieldDefinition,

        /// <summary>
        /// Location adjacent to an argument definition.
        /// </summary>
        ArgumentDefinition,

        /// <summary>
        /// Location adjacent to an interface definition.
        /// </summary>
        Interface,

        /// <summary>
        /// Location adjacent to a union definition.
        /// </summary>
        Union,

        /// <summary>
        /// Location adjacent to an enum definition.
        /// </summary>
        Enum,

        /// <summary>
        /// Location adjacent to an enum value definition.
        /// </summary>
        EnumValue,

        /// <summary>
        /// Location adjacent to an input object type definition.
        /// </summary>
        InputObject,

        /// <summary>
        /// Location adjacent to an input object field definition.
        /// </summary>
        InputFieldDefinition
    }
}