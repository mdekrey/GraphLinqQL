using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    internal class GraphQlInputField : Interfaces.__InputValue.GraphQlContract<GraphQlInputFieldInformation>
    {
        public override IGraphQlResult<string?> defaultValue()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> description()
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