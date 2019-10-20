using GraphLinqQL.Introspection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Introspection
{
    public class DirectiveDefinition : __Directive.GraphQlContract<DirectiveInformation>
    {
        public override IGraphQlObjectResult<IEnumerable<__InputValue>> Args() =>
            Original.Resolve(d => d.Arguments).List(_ => _.AsContract<GraphQlInputField>());

        public override IGraphQlScalarResult<string?> Description() =>
            Original.Resolve(v => v.Description);

        public override IGraphQlScalarResult<IEnumerable<__DirectiveLocation>> Locations() =>
            Original.Resolve(v => v.Locations.Select(ToInterfaceLocation));

        private __DirectiveLocation ToInterfaceLocation(DirectiveLocation arg)
        {
            return arg switch
            {
                DirectiveLocation.Query => __DirectiveLocation.Query,
                DirectiveLocation.Mutation => __DirectiveLocation.Mutation,
                DirectiveLocation.Subscription => __DirectiveLocation.Subscription,
                DirectiveLocation.Field => __DirectiveLocation.Field,
                DirectiveLocation.FragmentDefinition => __DirectiveLocation.FragmentDefinition,
                DirectiveLocation.FragmentSpread => __DirectiveLocation.FragmentSpread,
                DirectiveLocation.InlineFragment => __DirectiveLocation.InlineFragment,
                DirectiveLocation.VariableDefinition => __DirectiveLocation.VariableDefinition,
                DirectiveLocation.Schema => __DirectiveLocation.Schema,
                DirectiveLocation.Scalar => __DirectiveLocation.Scalar,
                DirectiveLocation.Object => __DirectiveLocation.Object,
                DirectiveLocation.FieldDefinition => __DirectiveLocation.FieldDefinition,
                DirectiveLocation.ArgumentDefinition => __DirectiveLocation.ArgumentDefinition,
                DirectiveLocation.Interface => __DirectiveLocation.Interface,
                DirectiveLocation.Union => __DirectiveLocation.Union,
                DirectiveLocation.Enum => __DirectiveLocation.Enum,
                DirectiveLocation.EnumValue => __DirectiveLocation.EnumValue,
                DirectiveLocation.InputObject => __DirectiveLocation.InputObject,
                DirectiveLocation.InputFieldDefinition => __DirectiveLocation.InputFieldDefinition,
                _ => throw new NotSupportedException()
            };
        }

        public override IGraphQlScalarResult<string> Name() =>
            Original.Resolve(v => v.Name);
    }
}
