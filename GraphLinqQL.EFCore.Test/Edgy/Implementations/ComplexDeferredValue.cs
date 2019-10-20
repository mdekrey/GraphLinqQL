using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexDeferredValue : Interfaces.Complex.GraphQlContract<int>
    {
        public override IGraphQlScalarResult<int> Integer() =>
            Original.Resolve(value => value).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<int>> Integers() =>
            Original.Resolve(value => Enumerable.Repeat(value, 5)).Defer(_ => _);

        public override IGraphQlScalarResult<int?> NullableInteger() =>
            Original.Resolve(value => (int?)value).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<int?>?> NullableIntegers() =>
            Original.Resolve(value => Enumerable.Repeat<int?>(value, 3).Concat(Enumerable.Repeat<int?>(null, 2))).Defer(_ => _);

        public override IGraphQlScalarResult<string> Text() =>
            Original.Resolve(value => value.ToString()).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<string>> Texts() =>
            Original.Resolve(value => Enumerable.Repeat(value.ToString(), 5)).Defer(_ => _);

        public override IGraphQlScalarResult<string?> NullableText() =>
            Original.Resolve(value => (string?)value.ToString()).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<string?>?> NullableTexts() =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2))).Defer(_ => _);

        public override IGraphQlObjectResult<Interfaces.Inner> Obj() =>
            Original.Resolve(value => value.ToString()).Defer(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> Objs() =>
            Original.Resolve(value => Enumerable.Repeat(value.ToString(), 5)).Defer(_ => _.List(_ => _.AsContract<Inner>()));

        public override IGraphQlObjectResult<Interfaces.Inner?> NullableObj() =>
            Original.Resolve(value => (string?)value.ToString()).Defer(_ => _.Nullable(_ => _.AsContract<Inner>()));

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> NullableObjs() =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)))
                .Defer(_ => _.Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>()))));
    }
}
