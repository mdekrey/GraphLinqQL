using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class FriendsConnection : Interfaces.FriendsConnection.GraphQlContract<FriendsConnection.Data>
    {
        public override IGraphQlObjectResult<IEnumerable<Interfaces.FriendsEdge?>?> edges() =>
            Original.Resolve(d => d.FilteredFriendIds).List(_ => _.AsContract<FriendsEdge>());

        public override IGraphQlObjectResult<IEnumerable<Character?>?> friends() =>
            Original.Resolve(d => d.FilteredFriendIds.Select(id => Domain.Data.humanLookup.ContainsKey(id) ? (object)Domain.Data.humanLookup[id] : Domain.Data.droidLookup[id]))
                .List(_ => _.AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()));

        public override IGraphQlObjectResult<Interfaces.PageInfo> pageInfo() =>
            Original.Resolve(d => d.PageInfo).AsContract<PageInfo>();

        public override IGraphQlScalarResult<int?> totalCount() =>
            Original.Resolve(d => (int?)d.friendIds.Length);

        public class Data
        {
            public readonly string[] friendIds;
            public readonly int first;
            public readonly int after;

            public Data(string[] friendIds, int? first, string? after)
            {
                this.friendIds = friendIds;
                this.first = first ?? 3;
                this.after = int.Parse(after ?? "0");
            }

            public IEnumerable<string> FilteredFriendIds => friendIds.Skip(after).Take(first);
            public PageInfo.Data PageInfo => new PageInfo.Data(startCursor: after.ToString(), endCursor: (after + first - 1).ToString(), hasNextPage: friendIds.Length > (after + first));
        }
    }
}
