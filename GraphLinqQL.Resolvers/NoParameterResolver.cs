using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    internal class NoParameterResolver : IGraphQlParameterResolver
    {
        public T GetParameter<T>(string parameter) => throw new KeyNotFoundException($"Key {parameter} not available in parameter list. Available parameters: [].");

        public string GetRawParameter(string parameter) => throw new KeyNotFoundException($"Key {parameter} not available in parameter list. Available parameters: [].");

        public bool HasParameter(string parameter) => false;
    }
}