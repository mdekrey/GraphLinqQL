﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQlResolver.StarWarsV4.Domain;
using GraphQlResolver.StarWarsV4.Interfaces;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    class Query : Interfaces.Root.GraphQlContract<GraphQlRoot>
    {
        private readonly StarWarsContext dbContext;

        public Query(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.Database.EnsureCreated();
        }

        public override IGraphQlResult<FilmsConnection?> allFilms(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<PeopleConnection?> allPeople(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<PlanetsConnection?> allPlanets(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<SpeciesConnection?> allSpecies(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<StarshipsConnection?> allStarships(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<VehiclesConnection?> allVehicles(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Film?> film(string? id, string? filmID)
        {
            var episodeId = int.Parse(id ?? filmID!);
            return Original.Resolve(_ => dbContext.Films.Where(film => film.EpisodeId == episodeId)).ConvertableList().As<Film>(f => f.FirstOrDefault());
        }

        public override IGraphQlResult<Node?> node(string id)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Person?> person(string? id, string? personID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Planet?> planet(string? id, string? planetID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Species?> species(string? id, string? speciesID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Starship?> starship(string? id, string? starshipID)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Vehicle?> vehicle(string? id, string? vehicleID)
        {
            throw new NotImplementedException();
        }
    }
}
