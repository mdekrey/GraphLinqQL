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
    }
}
