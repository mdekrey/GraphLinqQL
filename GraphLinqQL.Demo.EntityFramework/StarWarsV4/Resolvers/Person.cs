
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    internal class Person : Interfaces.Person.GraphQlContract<Domain.Person>
    {
        public override IGraphQlScalarResult<string?> birthYear(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> created(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> edited(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> eyeColor(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<PersonFilmsConnection?> filmConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> gender(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> hairColor(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<int?> height(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<Planet?> homeworld(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(p => p.Id.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

        public override IGraphQlScalarResult<double?> mass(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> name(FieldContext fieldContext) =>
            Original.Resolve(p => p.Name);

        public override IGraphQlScalarResult<string?> skinColor(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<Species?> species(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<PersonStarshipsConnection?> starshipConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlObjectResult<PersonVehiclesConnection?> vehicleConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }
    }
}