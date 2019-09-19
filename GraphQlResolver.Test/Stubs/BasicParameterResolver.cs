using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphQlResolver.Stubs
{
    internal class BasicParameterResolver : IGraphQlParameterResolver
    {
        private readonly IDictionary<string, string> parameters;

        public BasicParameterResolver(IDictionary<string, string> parameters)
        {
            this.parameters = parameters.ToImmutableDictionary();
        }

        public T GetParameter<T>(string parameter) => System.Text.Json.JsonSerializer.Deserialize<T>(GetRawParameter(parameter));

        public string GetRawParameter(string parameter) => parameters[parameter];

        public bool HasParameter(string parameter) => parameters.ContainsKey(parameter);
    }
}