using System;
using System.Collections.Generic;
using System.Linq;
using GraphQlResolver.Introspection.Interfaces;

namespace GraphQlResolver.Introspection
{
    public class GraphQlType : Interfaces.__Type.GraphQlContract<Type>
    {
        private readonly IServiceProvider serviceProvider;

        public GraphQlType(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            typeInformation = GraphQlJoin.Join<Type, IGraphQlTypeInformation>((originBase) => from t in originBase
                                                                                              let typeInfo = serviceProvider.Instantiate(GraphQlJoin.FindOriginal(t))
                                                                                              select GraphQlJoin.BuildPlaceholder(t, typeInfo));
        }

        private readonly GraphQlJoin<Type, IGraphQlTypeInformation> typeInformation; 


        public override IGraphQlResult<string?> description() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Description);

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
            Original.Join(typeInformation).Resolve((_, info) => info.Interfaces).ConvertableList().As<GraphQlType>();

        public override IGraphQlResult<__TypeKind> kind()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> name() =>
            Original.Join(typeInformation).Resolve((_, info) => info.Name);

        public override IGraphQlResult<__Type?> ofType() =>
            Original.Join(typeInformation).Resolve((_, info) => info.OfType).Convertable().As<GraphQlType>();

        public override IGraphQlResult<IEnumerable<__Type>?> possibleTypes() =>
            Original.Join(typeInformation).Resolve((_, info) => info.PossibleTypes).ConvertableList().As<GraphQlType>();
    }
}