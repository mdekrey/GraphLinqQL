using System.Collections.Generic;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlField : Interfaces.__Field.GraphQlContract<GraphQlFieldInformation>
    {
        public override IGraphQlObjectResult<IEnumerable<__InputValue>> Args() =>
            this.Resolve(field => field.Arguments).List(_ => _.AsContract<GraphQlInputField>());

        public override IGraphQlScalarResult<string?> DeprecationReason() =>
            this.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> Description() =>
            this.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> IsDeprecated() =>
            this.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> Name() =>
            this.Resolve(v => v.Name);

        public override IGraphQlObjectResult<__Type> Type() =>
            this.Resolve(v => v.FieldType).AsContract<GraphQlType>();
    }
}