using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromRef : Interfaces.WithInput.GraphQlContract<string>
    {
        public override IGraphQlObjectResult<Complex> Deferred() =>
            Original.Resolve(_ => _).AsContract<ComplexDeferredRef>();

        public override IGraphQlObjectResult<Complex> Plain() =>
            Original.Resolve(_ => _).AsContract<ComplexPlainRef>();

        public override IGraphQlObjectResult<Complex> Task() =>
            Original.Resolve(_ => _).AsContract<ComplexTaskRef>();
    }
}
