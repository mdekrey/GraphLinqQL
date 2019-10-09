using GraphLinqQL.Introspection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Introspection
{
    public class DirectiveDefinition : __Directive.GraphQlContract<DirectiveInformation>
    {
        public override IGraphQlResult<IEnumerable<__InputValue>> args(FieldContext fieldContext) =>
            Original.Resolve(d => d.Arguments).List(_ => _.AsContract<GraphQlInputField>());

        public override IGraphQlResult<string?> description(FieldContext fieldContext) =>
            Original.Resolve(v => v.Description);

        public override IGraphQlResult<IEnumerable<__DirectiveLocation>> locations(FieldContext fieldContext) =>
            Original.Resolve(v => v.Locations.Select(ToInterfaceLocation));

        private __DirectiveLocation ToInterfaceLocation(DirectiveLocation arg)
        {
            return arg switch
            {
                DirectiveLocation.Query => __DirectiveLocation.QUERY,
                DirectiveLocation.Mutation => __DirectiveLocation.MUTATION,
                DirectiveLocation.Subscription => __DirectiveLocation.SUBSCRIPTION,
                DirectiveLocation.Field => __DirectiveLocation.FIELD,
                DirectiveLocation.FragmentDefinition => __DirectiveLocation.FRAGMENT_DEFINITION,
                DirectiveLocation.FragmentSpread => __DirectiveLocation.FRAGMENT_SPREAD,
                DirectiveLocation.InlineFragment => __DirectiveLocation.INLINE_FRAGMENT,
                DirectiveLocation.VariableDefinition => __DirectiveLocation.VARIABLE_DEFINITION,
                DirectiveLocation.Schema => __DirectiveLocation.SCHEMA,
                DirectiveLocation.Scalar => __DirectiveLocation.SCALAR,
                DirectiveLocation.Object => __DirectiveLocation.OBJECT,
                DirectiveLocation.FieldDefinition => __DirectiveLocation.FIELD_DEFINITION,
                DirectiveLocation.ArgumentDefinition => __DirectiveLocation.ARGUMENT_DEFINITION,
                DirectiveLocation.Interface => __DirectiveLocation.INTERFACE,
                DirectiveLocation.Union => __DirectiveLocation.UNION,
                DirectiveLocation.Enum => __DirectiveLocation.ENUM,
                DirectiveLocation.EnumValue => __DirectiveLocation.ENUM_VALUE,
                DirectiveLocation.InputObject => __DirectiveLocation.INPUT_OBJECT,
                DirectiveLocation.InputFieldDefinition => __DirectiveLocation.INPUT_FIELD_DEFINITION,
                _ => throw new NotSupportedException()
            };
        }

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(v => v.Name);
    }
}
