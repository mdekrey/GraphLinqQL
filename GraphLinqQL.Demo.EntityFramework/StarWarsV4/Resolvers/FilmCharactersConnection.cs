using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<Pagination<Domain.FilmCharacter>>
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


        public override IGraphQlObjectResult<IEnumerable<Interfaces.Person?>?> characters(FieldContext fieldContext) =>
            Original.Resolve((c) => c.Paginated.Select(fc => fc.Character)).List(_ => _.AsContract<Person>());
        //Original.Resolve((c) => from fc in dbContext.FilmCharacters
        //                            where fc.EpisodeId == c.EpisodeId
        //                            select fc.Character).ConvertableList().As<Person>();

        public override IGraphQlObjectResult<IEnumerable<Interfaces.FilmCharactersEdge?>?> edges(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlObjectResult<PageInfo> pageInfo(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlScalarResult<int?> totalCount(FieldContext fieldContext) =>
            Original.Resolve((c) => (int?)(c.Unpaginated.Count()));
        //Original.Resolve((c) => (int?)(from fc in dbContext.FilmCharacters
        //                                   where fc.EpisodeId == c.EpisodeId
        //                                   select fc).Count());

    }
}
