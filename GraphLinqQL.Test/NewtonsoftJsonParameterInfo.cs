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
            return JsonConvert.DeserializeObject(json, t);
        }
    }
}