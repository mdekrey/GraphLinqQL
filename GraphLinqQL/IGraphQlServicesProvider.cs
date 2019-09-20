using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public interface IGraphQlServicesProvider
    {
        IGraphQlParameterResolverFactory GetParameterResolverFactory();
        IGraphQlResolvable GetResolverContract(Type contract);
        
    }
}
