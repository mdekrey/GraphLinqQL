using System.Collections.Generic;
#pragma warning disable CA1819 // Properties should not return arrays

namespace GraphLinqQL
{
    public class Then
    {

#nullable disable warnings
        public string? MatchResult { get; set; }
        public string[]? Sqlite { get; set; }
        public bool Passes { get; set; }
        public bool? CompilePasses { get; set; }
#nullable restore
    }
}
