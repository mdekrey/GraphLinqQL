using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Sample.Implementations
{
    class PageInfo : Interfaces.PageInfo.GraphQlContract<PageInfoValues>
    {
        public override IGraphQlScalarResult<string?> endCursor(FieldContext fieldContext)
        {
            return Original.ResolveTask(_ => _.EndCursor());
        }

        public override IGraphQlScalarResult<bool> hasNextPage(FieldContext fieldContext)
        {
            return Original.ResolveTask(_ => _.HasNextPage());
        }

        public override IGraphQlScalarResult<string?> startCursor(FieldContext fieldContext)
        {
            return Original.ResolveTask(_ => _.StartCursor());
        }
    }
}
