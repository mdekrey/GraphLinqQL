using System.Text;
using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlObjectResult<WithInput> FromReference() =>
            this.Resolve(_ => "foo").AsContract<WithInputFromRef>();

        public override IGraphQlObjectResult<WithInput> FromValue() =>
            this.Resolve(_ => 0).AsContract<WithInputFromValue>();

        public override IGraphQlObjectResult<Interfaces.Inner> Input(InputInner value) =>
            this.Resolve(_ => value.Value).AsContract<Inner>();

        public override IGraphQlObjectResult<Interfaces.Inner> InputWithCamelCase(WithCamelCase value) =>
            this.Resolve(_ => value.MyValue).AsContract<Inner>();

        public override IGraphQlObjectResult<Interfaces.Inner> InputWithUnderscores(WithUnderscores value) =>
            this.Resolve(_ => value.MyValue).AsContract<Inner>();
    }
}
