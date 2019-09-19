using System.Collections.Generic;
using GraphLinqQL.Introspection.Interfaces;

namespace GraphLinqQL.Introspection
{
    internal class GraphQlField : Interfaces.__Field.GraphQlContract<GraphQlFieldInformation>
    {
        public override IGraphQlResult<IEnumerable<__InputValue>> args() =>
            Original.Resolve(field => field.Arguments).List(_ => _.As<GraphQlInputField>());

        public override IGraphQlResult<string?> deprecationReason() =>
            Original.Resolve(v => v.DeprecationReason);

        public override IGraphQlResult<string?> description() =>
            Original.Resolve(v => v.Description);

        public override IGraphQlResult<bool> isDeprecated() =>
            Original.Resolve(v => v.IsDeprecated);

        public override IGraphQlResult<string> name() =>
            Original.Resolve(v => v.Name);

        public override IGraphQlResult<__Type> type() =>
            Original.Resolve(v => v.FieldType).As<GraphQlType>();
    }
}