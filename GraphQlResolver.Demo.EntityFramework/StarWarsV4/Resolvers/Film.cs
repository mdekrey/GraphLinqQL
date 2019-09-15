using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQlResolver.StarWarsV4.Interfaces;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    class Film : Interfaces.Film.GraphQlContract<Domain.Film>
    {
        public Film(Domain.StarWarsContext starWarsContext)
        {
        }

        public override IGraphQlResult<Interfaces.FilmCharactersConnection?> characterConnection(string? after, int? first, string? before, int? last)
        {
            // TODO - use parameters
            return Original.Resolve(film => film).Convertable().As<FilmCharactersConnectionFromFilm>();
        }

        public override IGraphQlResult<string?> created()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> director()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> edited()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> episodeID() =>
            Original.Resolve(film => (int?)film.EpisodeId);

        public override IGraphQlResult<string> id()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> openingCrawl()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<FilmPlanetsConnection?> planetConnection(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<string?>?> producers()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> releaseDate()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<FilmSpeciesConnection?> speciesConnection(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<FilmStarshipsConnection?> starshipConnection(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string?> title() =>
            Original.Resolve(film => film.Title);

        public override IGraphQlResult<FilmVehiclesConnection?> vehicleConnection(string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }
    }
}
