using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlSchema
{
    public class GraphQlId
    {
        public GraphQlId(string value) => Value = value;

        public string Value { get; }
    }
}
