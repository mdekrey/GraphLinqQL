using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromValue : Interfaces.WithInput.GraphQlContract<int>
    {
        public override IGraphQlObjectResult<Complex> deferred(FieldContext fieldContext) =>
            Original.Resolve(_ => _).AsContract<ComplexDeferredValue>();

        public override IGraphQlObjectResult<Complex> plain(FieldContext fieldContext) =>
            Original.Resolve(_ => _).AsContract<ComplexPlainValue>();

        public override IGraphQlObjectResult<Complex> task(FieldContext fieldContext) =>
            Original.Resolve(_ => _).AsContract<ComplexTaskValue>();

    }
}
