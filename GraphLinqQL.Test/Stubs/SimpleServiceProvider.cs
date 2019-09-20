using System;

namespace GraphLinqQL.Stubs
{
    class SimpleServiceProvider : IGraphQlServicesProvider
    {
        public IGraphQlParameterResolverFactory GetParameterResolverFactory()
        {
            return new BasicParameterResolverFactory();
        }

        public IGraphQlResolvable GetResolverContract(Type contract)
        {
            return (IGraphQlResolvable)Activator.CreateInstance(contract)!;
        }
    }

}
