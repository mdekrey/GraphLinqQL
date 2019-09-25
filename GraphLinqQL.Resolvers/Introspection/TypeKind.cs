namespace GraphLinqQL.Introspection
{
    public enum TypeKind
    {
        Scalar,
#pragma warning disable CA1720 // Identifier contains type name - generated from graphql
        Object,
#pragma warning restore CA1720 // Identifier contains type name
        Interface,
        Union,
        Enum,
        InputObject,
        List,
        NonNull
    }
}