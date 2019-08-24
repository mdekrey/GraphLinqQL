using System;
using GraphQlSchema;

namespace GraphQl.StarWarsSample
{
    public class SearchResult : GraphQlUnion
    {
        protected SearchResult(object value) 
            : base(value, typeof(Human), typeof(Droid), typeof(Starship))
        {
        }
    }
}