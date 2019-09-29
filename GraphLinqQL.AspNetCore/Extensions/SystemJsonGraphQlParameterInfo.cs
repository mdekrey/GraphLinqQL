﻿using GraphLinqQL;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Microsoft.AspNetCore.Builder
{
    internal readonly struct SystemJsonGraphQlParameterInfo : IGraphQlParameterInfo
    {
        static readonly FieldInfo? JsonDocumentField = typeof(JsonElement).GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly FieldInfo? JsonDocumentUtf8JsonField = typeof(JsonDocument).GetField("_utf8Json", BindingFlags.NonPublic | BindingFlags.Instance);

        ReadOnlyMemory<byte> Value { get; }
        JsonSerializerOptions? JsonSerializerOptions { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SystemJsonGraphQlParameterInfo(JsonElement jsonElement, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            if (JsonDocumentField == null) throw new InvalidOperationException(nameof(JsonDocumentField));
            if (JsonDocumentUtf8JsonField == null) throw new InvalidOperationException(nameof(JsonDocumentUtf8JsonField));
            var jsonDocument = JsonDocumentField.GetValue(jsonElement);
            Value = (ReadOnlyMemory<byte>)JsonDocumentUtf8JsonField.GetValue(jsonDocument)!;
            JsonSerializerOptions = jsonSerializerOptions;
        }

        public T BindTo<T>(IGraphQlParameterResolver variableResolver)
        {
            return (T)JsonSerializer.Deserialize(Value.Span, typeof(T), JsonSerializerOptions);
        }
    }
}
