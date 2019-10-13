using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.Introspection;

namespace GraphLinqQL
{
    public interface IGraphQlServiceProvider : IDisposable
    {
        IGraphQlResolvable GetResolverContract(Type contract);
        Introspection.IGraphQlTypeListing GetTypeListing();
        IGraphQlTypeResolver GetTypeResolver();
        IGraphQlTypeInformation? TryGetTypeInformation(Type? type);
    }
}
