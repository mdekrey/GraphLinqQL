using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromRef : Interfaces.WithInput.GraphQlContract<string>
    {
        public override IGraphQlObjectResult<Complex> Deferred() =>
            this.Resolve(_ => _).AsContract<ComplexDeferredRef>();

        public override IGraphQlObjectResult<Complex> Plain() =>
            this.Resolve(_ => _).AsContract<ComplexPlainRef>();

        public override IGraphQlObjectResult<Complex> Task() =>
            this.Resolve(_ => _).AsContract<ComplexTaskRef>();
    }
}
