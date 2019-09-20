using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlServiceProvider : IDisposable
    {
        IGraphQlParameterResolverFactory GetParameterResolverFactory();
        IGraphQlResolvable GetResolverContract(Type contract);
        Introspection.IGraphQlTypeListing GetTypeListing();
        IGraphQlTypeResolver? GetTypeResolver();
    }
}
