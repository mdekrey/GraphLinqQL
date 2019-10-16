using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Edgy.Implementations
{
    class Inner : Interfaces.Inner.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<string?> value(FieldContext fieldContext) =>
            Original.Resolve(_ => _);
    }
}
