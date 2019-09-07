using System;
using System.Collections.Generic;

namespace GraphQlResolver.Introspection
{
    public interface IGraphQlTypeInformation
    {
        string Name { get; }
        string? Description { get; }
        TypeKind Kind { get; }
        Type? OfType { get; }
        IReadOnlyList<Type>? Interfaces { get; }
        IReadOnlyList<Type>? PossibleTypes { get; }
    }
}