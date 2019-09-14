
namespace GraphQlResolver.StarWarsV4.Resolvers
{
    internal class Person : Interfaces.Person.GraphQlContract<Domain.Person>
    {
        public override IGraphQlResult<string?> birthYear()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> created()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> edited()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> eyeColor()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.PersonFilmsConnection?> filmConnection(string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> gender()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> hairColor()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<int?> height()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Planet?> homeworld()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string> id() =>
            Original.Resolve(p => p.Id.ToString());

        public override IGraphQlResult<double?> mass()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<string?> name() =>
            Original.Resolve(p => p.Name);

        public override IGraphQlResult<string?> skinColor()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Species?> species()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.PersonStarshipsConnection?> starshipConnection(string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.PersonVehiclesConnection?> vehicleConnection(string? after, int? first, string? before, int? last)
        {
            throw new System.NotImplementedException();
        }
    }
}