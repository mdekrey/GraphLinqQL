using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.StarWars.Implementations
{
    class PageInfo : Interfaces.PageInfo.GraphQlContract<PageInfoValues>
    {
        public override IGraphQlScalarResult<string?> endCursor()
        {
            return Original.ResolveTask(_ => _.EndCursor());
        }

        public override IGraphQlScalarResult<bool> hasNextPage()
        {
            return Original.ResolveTask(_ => _.HasNextPage());
        }

        public override IGraphQlScalarResult<string?> startCursor()
        {
            return Original.ResolveTask(_ => _.StartCursor());
        }
    }
}
