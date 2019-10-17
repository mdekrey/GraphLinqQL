using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromRef : Interfaces.WithInput.GraphQlContract<string>
    {
        public override IGraphQlObjectResult<Complex> deferred() =>
            Original.Resolve(_ => _).AsContract<ComplexDeferredRef>();

        public override IGraphQlObjectResult<Complex> plain() =>
            Original.Resolve(_ => _).AsContract<ComplexPlainRef>();

        public override IGraphQlObjectResult<Complex> task() =>
            Original.Resolve(_ => _).AsContract<ComplexTaskRef>();
    }
}
