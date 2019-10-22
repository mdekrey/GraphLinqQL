using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexDeferredRef : Interfaces.Complex.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<int> Integer() =>
            this.Resolve(value => 0).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<int>> Integers() =>
            this.Resolve(value => Enumerable.Repeat(0, 5)).Defer(_ => _);

        public override IGraphQlScalarResult<int?> NullableInteger() =>
            this.Resolve(value => (int?)0).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<int?>?> NullableIntegers() =>
            this.Resolve(value => Enumerable.Repeat<int?>(0, 3).Concat(Enumerable.Repeat<int?>(null, 2))).Defer(_ => _);

        public override IGraphQlScalarResult<string> Text() =>
            this.Resolve(value => value).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<string>> Texts() =>
            this.Resolve(value => Enumerable.Repeat(value, 5)).Defer(_ => _);

        public override IGraphQlScalarResult<string?> NullableText() =>
            this.Resolve(value => (string?)value).Defer(_ => _);

        public override IGraphQlScalarResult<IEnumerable<string?>?> NullableTexts() =>
            this.Resolve(value => Enumerable.Repeat<string?>(value, 3).Concat(Enumerable.Repeat<string?>(null, 2))).Defer(_ => _);

        public override IGraphQlObjectResult<Interfaces.Inner> Obj() =>
            this.Resolve(value => value).Defer(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> Objs() =>
            this.Resolve(value => Enumerable.Repeat(value, 5)).Defer(_ => _.List(_ => _.AsContract<Inner>()));

        public override IGraphQlObjectResult<Interfaces.Inner?> NullableObj() =>
            this.Resolve(value => (string?)value.ToString()).Defer(_ => _.Nullable(_ => _.AsContract<Inner>()));

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> NullableObjs() =>
            this.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)))
                .Defer(_ => _.Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>()))));
    }
}
