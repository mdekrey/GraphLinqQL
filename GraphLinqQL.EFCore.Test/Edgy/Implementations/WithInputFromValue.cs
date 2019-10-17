using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromValue : Interfaces.WithInput.GraphQlContract<int>
    {
        public override IGraphQlObjectResult<Complex> deferred() =>
            Original.Resolve(_ => _).AsContract<ComplexDeferredValue>();

        public override IGraphQlObjectResult<Complex> plain() =>
            Original.Resolve(_ => _).AsContract<ComplexPlainValue>();

        public override IGraphQlObjectResult<Complex> task() =>
            Original.Resolve(_ => _).AsContract<ComplexTaskValue>();

    }
}
