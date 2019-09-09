using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver.Introspection
{
    public class ListTypeInformation<T> : IGraphQlTypeInformation
        where T : IGraphQlTypeInformation
    {
        public string? Name => null;

        public string? Description => null;

        public TypeKind Kind => TypeKind.List;

        public Type? OfType => typeof(T);

        public IReadOnlyList<Type>? Interfaces => null;

        public IReadOnlyList<Type>? PossibleTypes => null;

        public IReadOnlyList<GraphQlInputFieldInformation>? InputFields => null;

        public IEnumerable<GraphQlEnumValueInformation>? EnumValues(bool? includeDeprecated) => null;

        public IEnumerable<GraphQlFieldInformation>? Fields(bool? includeDeprecated) => null;
    }
}
