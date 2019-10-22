using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphLinqQL.Execution
{
    internal class TypeConverterInputTypeConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IInputType).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(TypeConverterInput<>).MakeGenericType(typeToConvert))!;
        }
    }

    internal class TypeConverterInput<T> : JsonConverter<T>
        where T : IInputType, new()
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonTokenType token = reader.TokenType;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var value = new T();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return value;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string key = reader.GetString();

                reader.Read();
                Type localType = null!;
                value.SetValue(key, type =>
                {
                    localType = type;
                    return null;
                });
                var nextValue = JsonSerializer.Deserialize(ref reader, localType, options);
                value.SetValue(key, type => nextValue);
            }

            return default!;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // This is intended for input objects, so... writing them out isn't really a thing
            throw new NotSupportedException();
        }
    }
}