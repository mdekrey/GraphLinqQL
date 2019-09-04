﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQlResolver.StarWarsV3.Interfaces;

namespace GraphQlResolver.StarWarsV3.Resolvers
{
    class Droid : Interfaces.Droid.GraphQlContract<Domain.Droid>
    {
        public override IGraphQlResult<IEnumerable<Episode?>> appearsIn() =>
            Original.Resolve(droid => droid.AppearsIn.Select(DomainToInterface.ConvertEpisode).Select(ep => (Episode?)ep));

        public override IGraphQlResult<IEnumerable<Character?>?> friends()
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.FriendsConnection> friendsConnection(int? first, string? after)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<string> id() =>
            Original.Resolve(droid => droid.Id);

        public override IGraphQlResult<string> name() =>
            Original.Resolve(droid => droid.Name);

        public override IGraphQlResult<string?> primaryFunction() =>
            Original.Resolve(droid => droid.PrimaryFunction);
    }
}