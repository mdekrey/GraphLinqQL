using Newtonsoft.Json;
using System;

namespace GraphLinqQL
{
    internal class NewtonsoftJsonParameterInfo : IGraphQlParameterInfo
    {
        private string json;

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