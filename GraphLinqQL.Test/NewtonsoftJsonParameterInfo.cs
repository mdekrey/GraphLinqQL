using Newtonsoft.Json;

namespace GraphLinqQL
{
    internal class NewtonsoftJsonParameterInfo : IGraphQlParameterInfo
    {
        private string json;

        public NewtonsoftJsonParameterInfo(string json)
        {
            this.json = json;
        }

        public T BindTo<T>(IGraphQlParameterResolver variableResolver)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}