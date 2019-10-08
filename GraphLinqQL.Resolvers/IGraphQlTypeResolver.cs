using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlTypeResolver
    {
        Type ResolveForInput(string name);

        Type IntrospectionTypeListing { get; }
    }
}
