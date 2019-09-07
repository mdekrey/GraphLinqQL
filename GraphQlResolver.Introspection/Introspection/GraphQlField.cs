using System.Collections.Generic;
using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    internal class GraphQlField : Interfaces.__Field.GraphQlContract<GraphQlFieldInformation>
    {
        public override IGraphQlResult<IEnumerable<__InputValue>> args()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> deprecationReason()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> description()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<bool> isDeprecated()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string> name()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<__Type> type()
        {
            throw new System.NotImplementedException();
        }
    }
}