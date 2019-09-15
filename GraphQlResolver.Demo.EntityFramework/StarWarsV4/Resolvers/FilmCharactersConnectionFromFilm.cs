using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlResolver.StarWarsV4.Interfaces;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    public class FilmCharactersConnectionFromFilm : Interfaces.FilmCharactersConnection.GraphQlContract<Domain.Film>
    {
        private readonly GraphQlJoin<Domain.Film, IEnumerable<Domain.Person>> charactersJoin;

        public FilmCharactersConnectionFromFilm(Domain.StarWarsContext starWarsContext)
        {
            charactersJoin = GraphQlJoin.Join<Domain.Film, IEnumerable<Domain.Person>>((originBase) =>
                from t in originBase
                let characters = (from character in starWarsContext.FilmCharacters
                                  where GraphQlJoin.FindOriginal(t).EpisodeId == character.EpisodeId
                                  select character.Character).ToArray()
                select GraphQlJoin.BuildPlaceholder(t, (IEnumerable<Domain.Person>)characters));
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
