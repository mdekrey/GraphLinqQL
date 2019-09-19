using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    public class FilmsConnection : Interfaces.FilmsConnection.GraphQlContract<IQueryable<Domain.Film>>
    {
        public override IGraphQlResult<IEnumerable<FilmsEdge?>?> edges()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Film?>?> films()
        {
            return Original.Resolve(films => films).Nullable(_ => _.List(_ => _.As<Film>()));
        }

        public override IGraphQlResult<PageInfo> pageInfo()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> totalCount()
        {
            throw new NotImplementedException();
        }
    }
}
