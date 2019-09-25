using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.Execution
{
    public class SystemJsonGraphQlParameterResolverFactory : IGraphQlParameterResolverFactory
    {
        public IGraphQlParameterResolver FromParameterData(IDictionary<string, string> rawData)
        {
            return new SystemJsonGraphQlParameterResolver(rawData);
        }
    }

    internal class SystemJsonGraphQlParameterResolver : IGraphQlParameterResolver
    {
        private IDictionary<string, string> rawData;

        public SystemJsonGraphQlParameterResolver(IDictionary<string, string> rawData)
        {
            this.rawData = rawData;
        }

        public T GetParameter<T>(string parameter)
        {
            var value = rawData[parameter];
            if (typeof(T) == typeof(string))
            {
                // Because the main graphQl AST parser strips quotes from strings - FIXME - better GraphQL AST Parser - Issue #11
#pragma warning disable CA1305 // Specify IFormatProvider
                return (T)(object)value?.ToString()!;
#pragma warning restore CA1305 // Specify IFormatProvider
            }
            return System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }

        public string GetRawParameter(string parameter) => rawData[parameter];

        public bool HasParameter(string parameter) => rawData.ContainsKey(parameter);
    }
}
