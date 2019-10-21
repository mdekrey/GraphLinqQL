using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphLinqQL.Execution
{
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions GraphQlJsonSerializerOptions = Setup(new JsonSerializerOptions());

        public static JsonSerializerOptions Setup(JsonSerializerOptions jsonSerializerOptions)
        {
            jsonSerializerOptions.Converters.Add(new TypeConverterEnumConverter());
            return jsonSerializerOptions;
        }
    }
}
