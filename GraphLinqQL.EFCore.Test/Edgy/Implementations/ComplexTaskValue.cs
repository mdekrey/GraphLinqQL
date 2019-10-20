using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexTaskValue : Interfaces.Complex.GraphQlContract<int>
    {
        public override IGraphQlScalarResult<int> Integer() =>
            Original.ResolveTask(value => Task.FromResult(value));

        public override IGraphQlScalarResult<IEnumerable<int>> Integers() =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat(value, 5)));

        public override IGraphQlScalarResult<int?> NullableInteger() =>
            Original.ResolveTask(value => Task.FromResult((int?)value));

        public override IGraphQlScalarResult<IEnumerable<int?>?> NullableIntegers() =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat<int?>(value, 3).Concat(Enumerable.Repeat<int?>(null, 2))));

        public override IGraphQlScalarResult<string> Text() =>
            Original.ResolveTask(value => Task.FromResult(value.ToString()));

        public override IGraphQlScalarResult<IEnumerable<string>> Texts() =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat(value.ToString(), 5)));

        public override IGraphQlScalarResult<string?> NullableText() =>
            Original.ResolveTask(value => Task.FromResult((string?)value.ToString()));

        public override IGraphQlScalarResult<IEnumerable<string?>?> NullableTexts() =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2))));

        public override IGraphQlObjectResult<Interfaces.Inner> Obj() =>
            Original.ResolveTask(value => Task.FromResult(value.ToString())).AsContract<Inner>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> Objs() =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat(value.ToString(), 5))).List(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<Interfaces.Inner?> NullableObj() =>
            Original.ResolveTask(value => Task.FromResult((string?)value.ToString())).Nullable(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> NullableObjs() =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2))))
                .Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>())));
    }
}
