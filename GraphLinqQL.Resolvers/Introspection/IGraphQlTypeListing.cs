using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Introspection
{
    public interface IGraphQlTypeListing
    {
        Type Query { get; }
        Type? Mutation { get; }
        Type? Subscription { get; }

        IEnumerable<Type> TypeInformation { get; }
        IEnumerable<DirectiveInformation> DirectiveInformation { get; }

        Type? Type(string name);
    }
}
