using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL.StarWarsV4.Interfaces;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    public class FilmsConnection : Interfaces.FilmsConnection.GraphQlContract<IQueryable<Domain.Film>>
    {
        public override IGraphQlResult<IEnumerable<FilmsEdge?>?> edges(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Film?>?> films(FieldContext fieldContext)
        {
            return Original.Resolve(films => films).Nullable(_ => _.List(_ => _.AsContract<Film>()));
        }

        public override IGraphQlResult<PageInfo> pageInfo(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<int?> totalCount(FieldContext fieldContext)
        {
            throw new NotImplementedException();
        }
    }
}
