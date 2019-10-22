using GraphLinqQL.StarWars.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL.StarWars.Implementations
{
    class FriendsConnection : Interfaces.FriendsConnection.GraphQlContract<PaginatedSelection<Domain.Friendship>>
    {
        public override IGraphQlObjectResult<IEnumerable<Interfaces.FriendsEdge?>?> Edges()
        {
            return this.Resolve(_ => _.SkippedData.Take(_.Take)).List(_ => _.AsContract<FriendsEdge>());
        }

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Character?>?> Friends()
        {
            return this.Resolve(_ => from friendship in _.SkippedData.Take(_.Take)
                                         select friendship.To!)
                .List(_ => _.AsUnion<Interfaces.Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()));
        }

        public override IGraphQlObjectResult<Interfaces.PageInfo> PageInfo()
        {
            return this.ResolveTask(selection => Task.FromResult(new PageInfoValues
            {
                StartCursor = () => selection.SkippedData.Select(d => d.ToId.ToString()).FirstOrDefaultAsync(),
                EndCursor = () => selection.SkippedData.Select(d => d.ToId.ToString()).Take(selection.Take).LastOrDefaultAsync(),
                HasNextPage = () => selection.SkippedData.Skip(selection.Take).AnyAsync(),
            })).Defer(_ => _.AsContract<PageInfo>());
        }

        public override IGraphQlScalarResult<int?> TotalCount()
        {
            return this.Resolve(_ => (int?)_.AllData.Count());
        }
    }
}
