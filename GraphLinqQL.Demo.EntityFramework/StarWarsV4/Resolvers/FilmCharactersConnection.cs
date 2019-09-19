using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<IQueryable<Domain.FilmCharacter>>
    //public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<FilmCharactersConnection.Parameters>
    {
        //public class Parameters
        //{
        //    public int EpisodeId { get; }

        //    public Parameters(int episodeId, string? after, int? first, string? before, int? last)
        //    {
        //        this.EpisodeId = episodeId;
        //    }

        //}

        private readonly Domain.StarWarsContext dbContext;

        public FilmCharactersConnection(Domain.StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public override IGraphQlResult<IEnumerable<Interfaces.Person?>?> characters() =>
            Original.Resolve((c) => c.Select(fc => fc.Character)).List(_ => _.As<Person>());
        //Original.Resolve((c) => from fc in dbContext.FilmCharacters
        //                            where fc.EpisodeId == c.EpisodeId
        //                            select fc.Character).ConvertableList().As<Person>();

        public override IGraphQlResult<IEnumerable<Interfaces.FilmCharactersEdge?>?> edges()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<PageInfo> pageInfo()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> totalCount() =>
            Original.Resolve((c) => (int?)c.Count());
        //Original.Resolve((c) => (int?)(from fc in dbContext.FilmCharacters
        //                                   where fc.EpisodeId == c.EpisodeId
        //                                   select fc).Count());

    }
}
