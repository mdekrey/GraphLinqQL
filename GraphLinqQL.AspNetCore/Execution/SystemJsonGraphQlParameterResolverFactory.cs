using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.Execution
{
    public class SystemJsonGraphQlParameterResolverFactory : IGraphQlParameterResolverFactory
    {
        public IGraphQlParameterResolver FromParameterData(IDictionary<string, IGraphQlParameterInfo> rawData)
        {
            return new SystemJsonGraphQlParameterResolver(rawData);
        }
    }

    internal class SystemJsonGraphQlParameterResolver : IGraphQlParameterResolver
    {
        private IDictionary<string, IGraphQlParameterInfo> rawData;

        public SystemJsonGraphQlParameterResolver(IDictionary<string, IGraphQlParameterInfo> rawData)
        {
            this.rawData = rawData;
        }

        public T GetParameter<T>(string parameter) => (T)rawData[parameter].BindTo(typeof(T))!;

        public bool HasParameter(string parameter) => rawData.ContainsKey(parameter);
    }
}
