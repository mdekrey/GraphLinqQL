using System.Collections.Generic;
#pragma warning disable CA2227 // Collection properties should be read only

namespace GraphLinqQL
{
    public class Given
    {

#nullable disable warnings
        public string Query { get; set; }
        public string? Operation { get; set; }
        public Dictionary<string, object>? Variables { get; set; }
#nullable restore
    }
}
