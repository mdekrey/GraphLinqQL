using GraphLinqQL.Resolution;
using Newtonsoft.Json;
using System;

namespace GraphLinqQL
{
    internal class NewtonsoftJsonParameterInfo : IGraphQlParameterInfo
    {
        private readonly string json;

        public NewtonsoftJsonParameterInfo(string json)
        {
            this.json = json;
        }

        public object? BindTo(Type t)
        {
            // This is just a quick hack - it doesn't really handle Enums or InputTypes correctly
            return JsonConvert.DeserializeObject(json, t);
        }
    }
}