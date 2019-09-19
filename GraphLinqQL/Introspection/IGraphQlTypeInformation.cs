using System;
using System.Collections.Generic;

namespace GraphLinqQL.Introspection
{
    public interface IGraphQlTypeInformation
    {
        string? Name { get; }
        string? Description { get; }
        TypeKind Kind { get; }
        Type? OfType { get; }
        IReadOnlyList<Type>? Interfaces { get; }
        IReadOnlyList<Type>? PossibleTypes { get; }
        IReadOnlyList<GraphQlInputFieldInformation>? InputFields { get; }

        IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated);
        IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated);
    }
}