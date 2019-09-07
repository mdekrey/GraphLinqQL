using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver.Introspection
{
    public interface IGraphQlTypeListing
    {
        Type? Query { get; }
        Type? Mutation { get; }
        Type? Subscription { get; }

        IReadOnlyList<Type> TypeInformation { get; }

        Type? Type(string name);
    }
}
