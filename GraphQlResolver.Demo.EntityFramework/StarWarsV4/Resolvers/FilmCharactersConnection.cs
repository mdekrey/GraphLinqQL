using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<IEnumerable<Domain.FilmCharacter>>
    {
        private readonly GraphQlJoin<IEnumerable<Domain.FilmCharacter>, IEnumerable<Domain.Person>> charactersJoin;

        public FilmCharactersConnection(Domain.StarWarsContext starWarsContext)
        {
            charactersJoin = GraphQlJoin.Join<IEnumerable<Domain.FilmCharacter>, IEnumerable<Domain.Person>>((originBase) =>
                from t in originBase
                let characters = from character in starWarsContext.Characters
                                 from fc in GraphQlJoin.FindOriginal(t)
                                 where character.Id == fc.PersonId
                                 select character
                select GraphQlJoin.BuildPlaceholder(t, (IEnumerable<Domain.Person>)characters));
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Person?>?> characters() =>
            Original.Join(charactersJoin).Resolve((fc, characters) => characters).ConvertableList().As<Person>();

        public override IGraphQlResult<IEnumerable<Interfaces.FilmCharactersEdge?>?> edges()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.PageInfo> pageInfo()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> totalCount()
        {
            throw new NotImplementedException();
        }
    }
}
