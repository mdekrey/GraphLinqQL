using System.Text;
using GraphLinqQL.Edgy.Interfaces;

namespace GraphLinqQL.Edgy.Implementations
{
    class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlObjectResult<WithInput> fromRef(FieldContext fieldContext) =>
            Original.Resolve(_ => "foo").AsContract<WithInputFromRef>();

        public override IGraphQlObjectResult<WithInput> fromValue(FieldContext fieldContext) =>
            Original.Resolve(_ => 0).AsContract<WithInputFromValue>();
    }
}
