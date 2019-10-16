﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexTaskRef : Interfaces.Complex.GraphQlContract<string>
    {
        public override IGraphQlScalarResult<int> integer(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(0));

        public override IGraphQlScalarResult<IEnumerable<int>> integers(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat(0, 5)));

        public override IGraphQlScalarResult<int?> nullableInteger(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult((int?)0));

        public override IGraphQlScalarResult<IEnumerable<int?>?> nullableIntegers(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat<int?>(0, 3).Concat(Enumerable.Repeat<int?>(null, 2))));

        public override IGraphQlScalarResult<string> text(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(value));

        public override IGraphQlScalarResult<IEnumerable<string>> texts(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat(value, 5)));

        public override IGraphQlScalarResult<string?> nullableText(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult((string?)value));

        public override IGraphQlScalarResult<IEnumerable<string?>?> nullableTexts(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat<string?>(value, 3).Concat(Enumerable.Repeat<string?>(null, 2))));

        public override IGraphQlObjectResult<Interfaces.Inner> obj(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(value)).AsContract<Inner>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> objs(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat(value, 5))).List(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<Interfaces.Inner?> nullableObj(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult((string?)value.ToString())).Nullable(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> nullableObjs(FieldContext fieldContext) =>
            Original.ResolveTask(value => Task.FromResult(Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2))))
                .Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>())));
    }
}