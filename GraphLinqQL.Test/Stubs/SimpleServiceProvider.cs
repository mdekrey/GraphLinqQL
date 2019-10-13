using System;
using GraphLinqQL.Introspection;

namespace GraphLinqQL.Stubs
{
    class SimpleServiceProvider : IGraphQlServiceProvider
    {
        public void Dispose()
        {
        }

        public IGraphQlResolvable GetResolverContract(Type contract)
        {
            return (IGraphQlResolvable)Activator.CreateInstance(contract)!;
        }

        public IGraphQlTypeListing GetTypeListing()
        {
            throw new NotImplementedException();
        }

        public IGraphQlTypeResolver GetTypeResolver()
        {
            throw new NotImplementedException();
        }

        public IGraphQlTypeInformation? TryGetTypeInformation(Type? type)
        {
            throw new NotImplementedException();
        }
    }

}
