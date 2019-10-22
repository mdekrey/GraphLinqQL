using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.StarWars.Implementations
{
    class PageInfo : Interfaces.PageInfo.GraphQlContract<PageInfoValues>
    {
        public override IGraphQlScalarResult<string?> EndCursor()
        {
            return this.ResolveTask(_ => _.EndCursor());
        }

        public override IGraphQlScalarResult<bool> HasNextPage()
        {
            return this.ResolveTask(_ => _.HasNextPage());
        }

        public override IGraphQlScalarResult<string?> StartCursor()
        {
            return this.ResolveTask(_ => _.StartCursor());
        }
    }
}
