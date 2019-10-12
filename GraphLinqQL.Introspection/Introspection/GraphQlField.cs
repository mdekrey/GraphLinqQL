using System.Collections.Generic;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlField : Interfaces.__Field.GraphQlContract<GraphQlFieldInformation>
    {
        public override IGraphQlObjectResult<IEnumerable<__InputValue>> args(FieldContext fieldContext) =>
            Original.Resolve(field => field.Arguments).List(_ => _.AsContract<GraphQlInputField>());

        public override IGraphQlScalarResult<string?> deprecationReason(FieldContext fieldContext) =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlScalarResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(v => v.Description);

        public override IGraphQlScalarResult<bool> isDeprecated(FieldContext fieldContext) =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(v => v.Name);

        public override IGraphQlObjectResult<__Type> type(FieldContext fieldContext) =>
            Original.Resolve(v => v.FieldType).AsContract<GraphQlType>();
    }
}