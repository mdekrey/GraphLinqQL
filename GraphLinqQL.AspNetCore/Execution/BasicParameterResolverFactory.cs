using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Execution
{
    internal class BasicParameterResolverFactory : IGraphQlParameterResolverFactory
    {
        public IGraphQlParameterResolver FromParameterData(IDictionary<string, string> rawData)
        {
            return new BasicParameterResolver(rawData);
        }
    }
}
