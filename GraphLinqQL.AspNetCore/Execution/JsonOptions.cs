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
        public static readonly JsonSerializerOptions GraphQlJsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new TypeConverterEnumConverter() }
        };
    }
}
