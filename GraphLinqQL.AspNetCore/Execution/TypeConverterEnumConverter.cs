using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphLinqQL.Execution
{
    internal class TypeConverterEnumConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = TypeDescriptor.GetConverter(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(typeof(TypeConverterEnum<>).MakeGenericType(typeToConvert), converter)!;
        }
    }

    internal class TypeConverterEnum<T> : JsonConverter<T>
        where T : struct, Enum
    {
        private TypeConverter converter;

        public TypeConverterEnum(TypeConverter converter)
        {
            this.converter = converter;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonTokenType token = reader.TokenType;

            if (token == JsonTokenType.String)
            {
                // Try parsing case sensitive first
                string enumString = reader.GetString();
                return (T)converter.ConvertTo(enumString, typeof(T));
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var result = (string)converter.ConvertTo(value, typeof(string));
            writer.WriteStringValue(result);
        }
    }
}