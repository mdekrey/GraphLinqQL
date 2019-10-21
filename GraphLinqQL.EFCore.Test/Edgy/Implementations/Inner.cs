using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Edgy.Implementations
{
    class Inner : Interfaces.Inner.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<string?> NullableThrows() =>
            this.Original().ResolveTask<string, string?>(_ => throw new InvalidOperationException()).Nullable(_ => _);

        public override IGraphQlScalarResult<string> Throws() =>
            this.Original().ResolveTask<string, string>(_ => throw new InvalidOperationException());


        public override IGraphQlScalarResult<string?> Value() =>
            this.Original().Resolve(_ => _);
    }
}
