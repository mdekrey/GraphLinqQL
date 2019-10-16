using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    internal class WithInputFromRef : Interfaces.WithInput.GraphQlContract<string>
    {
        public override IGraphQlObjectResult<Complex> deferred(FieldContext fieldContext) =>
            Original.Resolve(_ => _).AsContract<ComplexDeferredRef>();

        public override IGraphQlObjectResult<Complex> plain(FieldContext fieldContext) =>
            Original.Resolve(_ => _).AsContract<ComplexPlainRef>();

        public override IGraphQlObjectResult<Complex> task(FieldContext fieldContext) =>
            Original.Resolve(_ => _).AsContract<ComplexTaskRef>();
    }
}
