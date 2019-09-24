using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.CommonTypes
{
#if !NET45
    [System.Text.Json.Serialization.JsonConverter(typeof(GraphQlIdConverter))]
#endif
    public class GraphQlId
    {
        public GraphQlId(string value) => Value = value;

        public string Value { get; }
    }
}
