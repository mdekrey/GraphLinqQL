using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlResolver.StarWarsV4.Interfaces;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    public class FilmCharactersConnectionFromFilm : Interfaces.FilmCharactersConnection.GraphQlContract<FilmCharactersConnectionFromFilm.ConnectionData>
    {
        public class ConnectionData
        {
            public readonly int episodeId;
            public readonly string? after;
            public readonly int? first;
            public readonly string? before;
            public readonly int? last;

            public ConnectionData(int episodeId, string? after, int? first, string? before, int? last)
            {
                this.episodeId = episodeId;
                this.after = after;
                this.first = first;
                this.before = before;
                this.last = last;
            }
        }

        private readonly GraphQlJoin<ConnectionData, IQueryable<Domain.Person>> charactersJoin;
        private readonly Domain.StarWarsContext dbContext;

        public FilmCharactersConnectionFromFilm(Domain.StarWarsContext dbContext)
        {
            charactersJoin = GraphQlJoin.JoinList<ConnectionData, Domain.Person>(film =>
                from character in dbContext.FilmCharacters
                where film.episodeId == character.EpisodeId
                select character.Character);
            this.dbContext = dbContext;
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Person?>?> characters() =>
            Original.Join(charactersJoin).Resolve((fc, characters) => characters).ConvertableList().As<Person>();

        public override IGraphQlResult<IEnumerable<Interfaces.FilmCharactersEdge?>?> edges()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<PageInfo> pageInfo()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> totalCount() =>
            Original.Join(charactersJoin).Resolve((fc, characters) => (int?)characters.Count());

    }
}
