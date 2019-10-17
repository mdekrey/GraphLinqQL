using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Edgy.Implementations
{
    class Inner : Interfaces.Inner.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<string?> nullableThrows() =>
            Original.ResolveTask<string, string?>(_ => throw new InvalidOperationException()).Nullable(_ => _);

        public override IGraphQlScalarResult<string> throws() =>
            Original.ResolveTask<string, string>(_ => throw new InvalidOperationException());


        public override IGraphQlScalarResult<string?> value() =>
            Original.Resolve(_ => _);
    }
}
