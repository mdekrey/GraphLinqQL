using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL.Stubs
{
    internal class BasicParameterResolver : IGraphQlParameterResolver
    {
        private readonly IDictionary<string, IGraphQlParameterInfo> parameters;

        public BasicParameterResolver(IDictionary<string, IGraphQlParameterInfo> parameters)
        {
            this.parameters = parameters.ToImmutableDictionary();
        }

        public T GetParameter<T>(string parameter) => parameters[parameter].BindTo<T>(this);

        public bool HasParameter(string parameter) => parameters.ContainsKey(parameter);
    }
}