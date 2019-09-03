using System;
using System.Text.Json;

namespace GraphQlResolver.CommonTypes
{
    internal class GraphQlIdConverter : System.Text.Json.Serialization.JsonConverter<GraphQlId>
    {
        public override GraphQlId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new GraphQlId(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, GraphQlId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}