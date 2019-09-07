using System;
using System.Collections.Generic;
using System.Linq;
using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    public class GraphQlType : Interfaces.__Type.GraphQlContract<IGraphQlTypeInformation>
    {
        private readonly IServiceProvider serviceProvider;

        public GraphQlType(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override IGraphQlResult<string?> description() =>
            Original.Resolve(info => info.Description);

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

        public override IGraphQlResult<IEnumerable<__Type>?> interfaces() =>
            Original.Resolve(info => info.Interfaces.Select(serviceProvider.Instantiate)).ConvertableList().As<GraphQlType>();

        public override IGraphQlResult<__TypeKind> kind()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> name() =>
            Original.Resolve(info => info.Name);

        public override IGraphQlResult<__Type?> ofType() =>
            Original.Resolve(info => serviceProvider.MaybeInstantiate(info.OfType)).Convertable().As<GraphQlType>();

        public override IGraphQlResult<IEnumerable<__Type>?> possibleTypes() =>
            Original.Resolve(info => info.PossibleTypes.Select(serviceProvider.Instantiate)).ConvertableList().As<GraphQlType>();
    }
}