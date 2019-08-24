using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace GraphQlSchema
{
    public class GraphQlUnion
    {
        protected GraphQlUnion(object value, params Type[] allowedTypes)
        {
            var valueType = value.GetType();
            if (!allowedTypes.Any(t => t.IsAssignableFrom(valueType)))
            {
                throw new ArgumentException($"{nameof(value)} must be a type matching one of the {nameof(allowedTypes)}", nameof(value));
            }
            this.Value = value;
        }

        public object Value { get; }
    }
}
