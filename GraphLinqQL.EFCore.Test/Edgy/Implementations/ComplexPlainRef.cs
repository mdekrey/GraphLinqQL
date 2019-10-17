using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexPlainRef : Interfaces.Complex.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<int> integer() =>
            Original.Resolve(value => 0);

        public override IGraphQlScalarResult<IEnumerable<int>> integers() =>
            Original.Resolve(value => Enumerable.Repeat(0, 5));

        public override IGraphQlScalarResult<int?> nullableInteger() =>
            Original.Resolve(value => (int?)0);

        public override IGraphQlScalarResult<IEnumerable<int?>?> nullableIntegers() =>
            Original.Resolve(value => Enumerable.Repeat<int?>(0, 3).Concat(Enumerable.Repeat<int?>(null, 2)));

        public override IGraphQlScalarResult<string> text() =>
            Original.Resolve(value => value);

        public override IGraphQlScalarResult<IEnumerable<string>> texts() =>
            Original.Resolve(value => Enumerable.Repeat(value, 5));

        public override IGraphQlScalarResult<string?> nullableText() =>
            Original.Resolve(value => (string?)value);

        public override IGraphQlScalarResult<IEnumerable<string?>?> nullableTexts() =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value, 3).Concat(Enumerable.Repeat<string?>(null, 2)));

        public override IGraphQlObjectResult<Interfaces.Inner> obj() =>
            Original.Resolve(value => value).AsContract<Inner>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> objs() =>
            Original.Resolve(value => Enumerable.Repeat(value, 5)).List(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<Interfaces.Inner?> nullableObj() =>
            Original.Resolve(value => (string?)value.ToString()).Nullable(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> nullableObjs() =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)))
                .Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>())));
    }
}
