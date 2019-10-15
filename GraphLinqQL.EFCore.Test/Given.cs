using System.Collections.Generic;
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CA2227 // Collection properties should be read only

namespace GraphLinqQL
{
    public class Given
    {

#nullable disable warnings
        public string? SetupQuery { get; set; }
        public string Query { get; set; }
        public string? Operation { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
#nullable restore
    }
}
