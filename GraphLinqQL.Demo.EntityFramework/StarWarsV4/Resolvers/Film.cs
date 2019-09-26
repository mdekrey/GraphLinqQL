﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    class Film : Interfaces.Film.GraphQlContract<Domain.Film>
    {
        private readonly Domain.StarWarsContext dbContext;

        public Film(Domain.StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlResult<Interfaces.FilmCharactersConnection?> characterConnection(string? after, int? first, string? before, int? last)
        {
            var query = from fc in dbContext.FilmCharacters
                        orderby fc.PersonId
                        select fc;
            if (after != null || first != null || (before == null && last == null))
            {
                var take = first ?? 10;
                if (after == null)
                {
                    return Original.Defer(_ => _.Resolve(film => query.Where(fc => fc.EpisodeId == film.EpisodeId).Paginate(q => q.Take(take)))).AsContract<FilmCharactersConnection>();
                }
                else
                {
                    var id = int.Parse(after, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    return (Original.Defer(_ => _.Resolve(film => query.Where(fc => fc.EpisodeId == film.EpisodeId).Paginate(q => q.Where(c => c.PersonId > id).Take(take)))).AsContract<FilmCharactersConnection>());
                }
            }
            else
            {
                var take = last ?? 10;
                if (before == null)
                {
                    return (Original.Defer(_ => _.Resolve(film => query.Where(fc => fc.EpisodeId == film.EpisodeId).OrderByDescending(c => c.PersonId).Paginate(q => q.Take(take)))).AsContract<FilmCharactersConnection>());
                }
                else
                {
                    var id = int.Parse(before, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    return (Original.Defer(_ => _.Resolve(film => query.Where(fc => fc.EpisodeId == film.EpisodeId).OrderByDescending(c => c.PersonId).Paginate(q => q.Where(c => c.PersonId < id).Take(take)))).AsContract<FilmCharactersConnection>());
                }
            }
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
