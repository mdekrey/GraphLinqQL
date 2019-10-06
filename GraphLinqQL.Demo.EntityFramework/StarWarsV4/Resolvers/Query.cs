using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWarsV4.Domain;
using GraphLinqQL.StarWarsV4.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    class Query : Interfaces.Root.GraphQlContract<GraphQlRoot>
    {
        private readonly StarWarsContext dbContext;

        public Query(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.Database.EnsureCreated();
        }

        public override IGraphQlResult<Interfaces.FilmsConnection?> allFilms(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            return Original.Resolve(_ => (IQueryable<Domain.Film>)dbContext.Films.AsNoTracking()).AsContract<FilmsConnection>();
        }

        public override IGraphQlResult<PeopleConnection?> allPeople(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<PlanetsConnection?> allPlanets(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<SpeciesConnection?> allSpecies(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<StarshipsConnection?> allStarships(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<VehiclesConnection?> allVehicles(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Film?> film(FieldContext fieldContext, string? id, string? filmID)
        {
            var episodeId = int.Parse(id ?? filmID!, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            return Original.Resolve(_ => dbContext.Films.Where(film => film.EpisodeId == episodeId)).Nullable(_ => _.List(_ => _.AsContract<Film>()).Only());
        }

        public override IGraphQlResult<Node?> node(FieldContext fieldContext, string id)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Person?> person(FieldContext fieldContext, string? id, string? personID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Planet?> planet(FieldContext fieldContext, string? id, string? planetID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Species?> species(FieldContext fieldContext, string? id, string? speciesID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Starship?> starship(FieldContext fieldContext, string? id, string? starshipID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Vehicle?> vehicle(FieldContext fieldContext, string? id, string? vehicleID)
        {
            throw new NotImplementedException();
        }
    }
}
