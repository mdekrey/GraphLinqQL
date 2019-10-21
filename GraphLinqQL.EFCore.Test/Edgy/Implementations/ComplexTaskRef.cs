using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexTaskRef : Interfaces.Complex.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<int> Integer() =>
            this.Original().ResolveTask(value => Task.FromResult(0));

        public override IGraphQlScalarResult<IEnumerable<int>> Integers() =>
            this.Original().ResolveTask(value => Task.FromResult(Enumerable.Repeat(0, 5)));

        public override IGraphQlScalarResult<int?> NullableInteger() =>
            this.Original().ResolveTask(value => Task.FromResult((int?)0));

        public override IGraphQlScalarResult<IEnumerable<int?>?> NullableIntegers() =>
            this.Original().ResolveTask(value => Task.FromResult(Enumerable.Repeat<int?>(0, 3).Concat(Enumerable.Repeat<int?>(null, 2))));

        public override IGraphQlScalarResult<string> Text() =>
            this.Original().ResolveTask(value => Task.FromResult(value));

        public override IGraphQlScalarResult<IEnumerable<string>> Texts() =>
            this.Original().ResolveTask(value => Task.FromResult(Enumerable.Repeat(value, 5)));

        public override IGraphQlScalarResult<string?> NullableText() =>
            this.Original().ResolveTask(value => Task.FromResult((string?)value));

        public override IGraphQlScalarResult<IEnumerable<string?>?> NullableTexts() =>
            this.Original().ResolveTask(value => Task.FromResult(Enumerable.Repeat<string?>(value, 3).Concat(Enumerable.Repeat<string?>(null, 2))));

        public override IGraphQlObjectResult<Interfaces.Inner> Obj() =>
            this.Original().ResolveTask(value => Task.FromResult(value)).AsContract<Inner>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> Objs() =>
            this.Original().ResolveTask(value => Task.FromResult(Enumerable.Repeat(value, 5))).List(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<Interfaces.Inner?> NullableObj() =>
            this.Original().ResolveTask(value => Task.FromResult((string?)value.ToString())).Nullable(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> NullableObjs() =>
            this.Original().ResolveTask(value => Task.FromResult(Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2))))
                .Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>())));
    }
}
