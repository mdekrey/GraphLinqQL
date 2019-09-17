using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlResolver.StarWarsV4.Interfaces;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<IQueryable<Domain.FilmCharacter>>
    {
        public override IGraphQlResult<IEnumerable<Interfaces.Person?>?> characters() =>
            Original.Resolve((c) => c.Select(fc => fc.Character)).ConvertableList().As<Person>();

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

    }
}
