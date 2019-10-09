
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    internal class Person : Interfaces.Person.GraphQlContract<Domain.Person>
    {
        public override IGraphQlResult<string?> birthYear(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> created(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> edited(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> eyeColor(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<PersonFilmsConnection?> filmConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> gender(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> hairColor(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<int?> height(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Planet?> homeworld(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(p => p.Id.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

        public override IGraphQlResult<double?> mass(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> name(FieldContext fieldContext) =>
            Original.Resolve(p => p.Name);

        public override IGraphQlResult<string?> skinColor(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Species?> species(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<PersonStarshipsConnection?> starshipConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<PersonVehiclesConnection?> vehicleConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }
    }
}