using System;
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

        public override IGraphQlObjectResult<Interfaces.FilmCharactersConnection?> characterConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
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

        public override IGraphQlScalarResult<string?> created(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> director(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> edited(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<int?> episodeID(FieldContext fieldContext) =>
            Original.Resolve(film => (int?)film.EpisodeId);

        public override IGraphQlScalarResult<string> id(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> openingCrawl(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<FilmPlanetsConnection?> planetConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<IEnumerable<string?>?> producers(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> releaseDate(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<FilmSpeciesConnection?> speciesConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<FilmStarshipsConnection?> starshipConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<string?> title(FieldContext fieldContext) =>
            Original.Resolve(film => film.Title);

        public override IGraphQlObjectResult<FilmVehiclesConnection?> vehicleConnection(FieldContext fieldContext, string? after, int? first, string? before, int? last)
        {
            throw new NotImplementedException();
        }
    }
}
