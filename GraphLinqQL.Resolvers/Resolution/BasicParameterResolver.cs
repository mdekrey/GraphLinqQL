﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL.Resolution
{
    public class BasicParameterResolver : IGraphQlParameterResolver
    {
        public static readonly IGraphQlParameterResolver Empty = new BasicParameterResolver(ImmutableDictionary<string, IGraphQlParameterInfo>.Empty);

        private readonly IDictionary<string, IGraphQlParameterInfo> parameters;

        public BasicParameterResolver(IDictionary<string, IGraphQlParameterInfo> parameters)
        {
            this.parameters = parameters.ToImmutableDictionary();
        }

        public T GetParameter<T>(string parameter) => parameters.ContainsKey(parameter)
            ? (T)parameters[parameter].BindTo(typeof(T))!
            : throw new ArgumentException("Missing argument on GraphQL query").AddGraphQlError(WellKnownErrorCodes.MissingArgument, EmptyArrayHelper.Empty<QueryLocation>(), new { argument = parameter });

        public bool HasParameter(string parameter) => parameters.ContainsKey(parameter);
    }
}