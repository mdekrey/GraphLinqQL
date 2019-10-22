using System;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexPlainValue : Interfaces.Complex.GraphQlContract<int>
    {
        public override IGraphQlScalarResult<int> Integer() =>
            this.Resolve(value => value);

        public override IGraphQlScalarResult<IEnumerable<int>> Integers() =>
            this.Resolve(value => Enumerable.Repeat(value, 5));

        public override IGraphQlScalarResult<int?> NullableInteger() =>
            this.Resolve(value => (int?)value);

        public override IGraphQlScalarResult<IEnumerable<int?>?> NullableIntegers() =>
            this.Resolve(value => Enumerable.Repeat<int?>(value, 3).Concat(Enumerable.Repeat<int?>(null, 2)));

        public override IGraphQlScalarResult<string> Text() =>
            this.Resolve(value => value.ToString());

        public override IGraphQlScalarResult<IEnumerable<string>> Texts() =>
            this.Resolve(value => Enumerable.Repeat(value.ToString(), 5));

        public override IGraphQlScalarResult<string?> NullableText() =>
            this.Resolve(value => (string?)value.ToString());

        public override IGraphQlScalarResult<IEnumerable<string?>?> NullableTexts() =>
            this.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)));

        public override IGraphQlObjectResult<Interfaces.Inner> Obj() =>
            this.Resolve(value => value.ToString()).AsContract<Inner>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> Objs() =>
            this.Resolve(value => Enumerable.Repeat(value.ToString(), 5)).List(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<Interfaces.Inner?> NullableObj() =>
            this.Resolve(value => (string?)value.ToString()).Nullable(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> NullableObjs() =>
            this.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)))
                .Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>())));

    }
}
