using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Edgy.Implementations
{
    class Inner : Interfaces.Inner.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<string?> NullableThrows() =>
            Original.ResolveTask<string, string?>(_ => throw new InvalidOperationException()).Nullable(_ => _);

        public override IGraphQlScalarResult<string> Throws() =>
            Original.ResolveTask<string, string>(_ => throw new InvalidOperationException());


        public override IGraphQlScalarResult<string?> Value() =>
            Original.Resolve(_ => _);
    }
}
