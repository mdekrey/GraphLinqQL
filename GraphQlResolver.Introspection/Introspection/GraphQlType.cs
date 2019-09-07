using System;
using System.Collections.Generic;
using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    public class GraphQlType : Interfaces.__Type.GraphQlContract<IGraphQlTypeInformation>
    {
        public override IGraphQlResult<string?> description()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<__EnumValue>?> enumValues(bool? includeDeprecated)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<__Field>?> fields(bool? includeDeprecated)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<__InputValue>?> inputFields()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<__Type>?> interfaces()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<__TypeKind> kind()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> name() =>
            Original.Resolve(info => info.Name);

        public override IGraphQlResult<__Type?> ofType()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<__Type>?> possibleTypes()
        {
            throw new NotImplementedException();
        }
    }
}