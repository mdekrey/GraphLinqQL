﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class ComplexPlainValue : Interfaces.Complex.GraphQlContract<int>
    {
        public override IGraphQlScalarResult<int> integer(FieldContext fieldContext) =>
            Original.Resolve(value => value);

        public override IGraphQlScalarResult<IEnumerable<int>> integers(FieldContext fieldContext) =>
            Original.Resolve(value => Enumerable.Repeat(value, 5));

        public override IGraphQlScalarResult<int?> nullableInteger(FieldContext fieldContext) =>
            Original.Resolve(value => (int?)value);

        public override IGraphQlScalarResult<IEnumerable<int?>?> nullableIntegers(FieldContext fieldContext) =>
            Original.Resolve(value => Enumerable.Repeat<int?>(value, 3).Concat(Enumerable.Repeat<int?>(null, 2)));

        public override IGraphQlScalarResult<string> text(FieldContext fieldContext) =>
            Original.Resolve(value => value.ToString());

        public override IGraphQlScalarResult<IEnumerable<string>> texts(FieldContext fieldContext) =>
            Original.Resolve(value => Enumerable.Repeat(value.ToString(), 5));

        public override IGraphQlScalarResult<string?> nullableText(FieldContext fieldContext) =>
            Original.Resolve(value => (string?)value.ToString());

        public override IGraphQlScalarResult<IEnumerable<string?>?> nullableTexts(FieldContext fieldContext) =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)));

        public override IGraphQlObjectResult<Interfaces.Inner> obj(FieldContext fieldContext) =>
            Original.Resolve(value => value.ToString()).AsContract<Inner>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner>> objs(FieldContext fieldContext) =>
            Original.Resolve(value => Enumerable.Repeat(value.ToString(), 5)).List(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<Interfaces.Inner?> nullableObj(FieldContext fieldContext) =>
            Original.Resolve(value => (string?)value.ToString()).Nullable(_ => _.AsContract<Inner>());

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Inner?>?> nullableObjs(FieldContext fieldContext) =>
            Original.Resolve(value => Enumerable.Repeat<string?>(value.ToString(), 3).Concat(Enumerable.Repeat<string?>(null, 2)))
                .Nullable(_ => _.List(_ => _.Nullable(_ => _.AsContract<Inner>())));

    }
}