using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexDeferredValue : Interfaces.Complex.GraphQlContract<int>
    {
        public override IGraphQlScalarResult<int> integer() =>
            Original.Resolve(value => value).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<int>> integers() =>
            Original.Resolve(value => Enumerable.Repeat(value, 5)).Defer(_ => _);

        public override IGraphQlScalarResult<int?> nullableInteger() =>
            Original.Resolve(value => (int?)value).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<int?>?> nullableIntegers() =>
            Original.Resolve(value => Enumerable.Repeat<int?>(value, 3).Concat(Enumerable.Repeat<int?>(null, 2))).Defer(_ => _);

        public override IGraphQlScalarResult<string> text() =>
            Original.Resolve(value => value.ToString()).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<string>> texts() =>
            Original.Resolve(value => Enumerable.Repeat(value.ToString(), 5)).Defer(_ => _);

        public override IGraphQlScalarResult<string?> nullableText() =>
            Original.Resolve(value => (string?)value.ToString()).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<string?>?> nullableTexts() =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2))).Defer(_ => _);

        public override IGraphQlObjectResult<Interfaces.Inner> obj() =>
            Original.Resolve(value => value.ToString()).Defer(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> objs() =>
            Original.Resolve(value => Enumerable.Repeat(value.ToString(), 5)).Defer(_ => _.List(_ => _.AsContract<Inner>()));

        public override IGraphQlObjectResult<Interfaces.Inner?> nullableObj() =>
            Original.Resolve(value => (string?)value.ToString()).Defer(_ => _.Nullable(_ => _.AsContract<Inner>()));

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> nullableObjs() =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)))
                .Defer(_ => _.Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>()))));
    }
}
