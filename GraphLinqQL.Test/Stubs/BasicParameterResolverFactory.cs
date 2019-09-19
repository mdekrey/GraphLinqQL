using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Stubs
{
    internal class BasicParameterResolverFactory : IGraphQlParameterResolverFactory
    {
        public IGraphQlParameterResolver FromParameterData(IDictionary<string, string> rawData)
        {
            return new BasicParameterResolver(rawData);
        }
    }
}
