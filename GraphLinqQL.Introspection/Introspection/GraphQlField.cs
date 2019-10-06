using System.Collections.Generic;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlField : Interfaces.__Field.GraphQlContract<GraphQlFieldInformation>
    {
        public override IGraphQlResult<IEnumerable<__InputValue>> args(FieldContext fieldContext) =>
            Original.Resolve(field => field.Arguments).List(_ => _.AsContract<GraphQlInputField>());

        public override IGraphQlResult<string?> deprecationReason(FieldContext fieldContext) =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(v => v.Description);

        public override IGraphQlResult<bool> isDeprecated(FieldContext fieldContext) =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(v => v.Name);

        public override IGraphQlResult<__Type> type(FieldContext fieldContext) =>
            Original.Resolve(v => v.FieldType).AsContract<GraphQlType>();
    }
}