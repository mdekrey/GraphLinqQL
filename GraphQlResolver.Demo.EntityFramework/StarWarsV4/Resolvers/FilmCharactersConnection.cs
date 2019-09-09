using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<IEnumerable<Domain.FilmCharacter>>
    {
        private static readonly GraphQlJoin<IEnumerable<Domain.FilmCharacter>, IEnumerable<Domain.Person>> charactersJoin =
            GraphQlJoin.Join<IEnumerable<Domain.FilmCharacter>, IEnumerable<Domain.Person>>((originBase) => from t in originBase
                                                                                                            let characters = from fc in GraphQlJoin.FindOriginal(t)
                                                                                                                             select fc.Character
                                                                                             select GraphQlJoin.BuildPlaceholder(t, characters));

        public override IGraphQlResult<IEnumerable<Interfaces.Person?>?> characters() =>
            Original.Resolve(fcs => from fc in fcs
                                    select fc.Character).ConvertableList().As<Person>();

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
