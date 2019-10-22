using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromValue : Interfaces.WithInput.GraphQlContract<int>
    {
        public override IGraphQlObjectResult<Complex> Deferred() =>
            this.Resolve(_ => _).AsContract<ComplexDeferredValue>();

        public override IGraphQlObjectResult<Complex> Plain() =>
            this.Resolve(_ => _).AsContract<ComplexPlainValue>();

        public override IGraphQlObjectResult<Complex> Task() =>
            this.Resolve(_ => _).AsContract<ComplexTaskValue>();

    }
}
