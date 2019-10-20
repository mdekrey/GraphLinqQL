using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromValue : Interfaces.WithInput.GraphQlContract<int>
    {
        public override IGraphQlObjectResult<Complex> Deferred() =>
            Original.Resolve(_ => _).AsContract<ComplexDeferredValue>();

        public override IGraphQlObjectResult<Complex> Plain() =>
            Original.Resolve(_ => _).AsContract<ComplexPlainValue>();

        public override IGraphQlObjectResult<Complex> Task() =>
            Original.Resolve(_ => _).AsContract<ComplexTaskValue>();

    }
}
