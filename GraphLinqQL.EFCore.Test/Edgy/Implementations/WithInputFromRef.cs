using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromRef : Interfaces.WithInput.GraphQlContract<string>
    {
        public override IGraphQlObjectResult<Complex> Deferred() =>
            this.Original().Resolve(_ => _).AsContract<ComplexDeferredRef>();

        public override IGraphQlObjectResult<Complex> Plain() =>
            this.Original().Resolve(_ => _).AsContract<ComplexPlainRef>();

        public override IGraphQlObjectResult<Complex> Task() =>
            this.Original().Resolve(_ => _).AsContract<ComplexTaskRef>();
    }
}
