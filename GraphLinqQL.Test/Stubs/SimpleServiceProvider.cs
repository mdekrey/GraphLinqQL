using System;

namespace GraphLinqQL.Stubs
{
    class SimpleServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IGraphQlParameterResolverFactory))
            {
                return new BasicParameterResolverFactory();
            }
            else
            {
                try
                {
                    return Activator.CreateInstance(serviceType);
                }
                catch
                {
                    return null;
                }
            }
        }
    }

}
