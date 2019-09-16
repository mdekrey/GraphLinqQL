using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlResolver.StarWarsV4.Interfaces;

namespace GraphQlResolver.StarWarsV4.Resolvers
{
    public class FilmCharactersConnection : Interfaces.FilmCharactersConnection.GraphQlContract<FilmCharactersConnection.ConnectionData>
    {
        public class ConnectionData
        {
            public ConnectionData(IQueryable<Domain.Person> people, string cursorData)
            {
                People = people;
                CursorData = cursorData;
            }

            public IQueryable<Domain.Person> People { get; }
            public string CursorData { get; }
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Person?>?> characters() =>
            Original.Resolve((fc) => fc.People).ConvertableList().As<Person>();

        public override IGraphQlResult<IEnumerable<Interfaces.FilmCharactersEdge?>?> edges()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<PageInfo> pageInfo()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> totalCount() =>
            Original.Resolve((fc) => (int?)fc.People.Count());

    }
}
