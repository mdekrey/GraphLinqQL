using System.Collections.Generic;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlField : Interfaces.__Field.GraphQlContract<GraphQlFieldInformation>
    {
        public override IGraphQlObjectResult<IEnumerable<__InputValue>> Args() =>
            Original.Resolve(field => field.Arguments).List(_ => _.AsContract<GraphQlInputField>());

        public override IGraphQlScalarResult<string?> DeprecationReason() =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> Description() =>
            Original.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> IsDeprecated() =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> Name() =>
            Original.Resolve(v => v.Name);

        public override IGraphQlObjectResult<__Type> Type() =>
            Original.Resolve(v => v.FieldType).AsContract<GraphQlType>();
    }
}