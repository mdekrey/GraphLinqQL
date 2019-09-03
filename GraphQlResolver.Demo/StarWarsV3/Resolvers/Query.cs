using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GraphQlResolver.StarWarsV3.Interfaces;

namespace GraphQlResolver.StarWarsV3.Resolvers
{
    class Query : Interfaces.Query.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlResult<Character?> character(string id) =>
            Domain.Data.humanLookup.TryGetValue(id, out var human) ? Original.Resolve(_ => human).Convertable().As<Human>()
            : Domain.Data.droidLookup.TryGetValue(id, out var droid) ? Original.Resolve(_ => droid).Convertable().As<Droid>()
            : Original.Resolve(_ => (Character?)null);

        public override IGraphQlResult<Interfaces.Droid?> droid(string id) =>
            Original.Resolve(_ => Domain.Data.droidLookup[id]).Convertable().As<Droid>();

        public override IGraphQlResult<Character?> hero(Episode? episode) =>
            episode == Episode.EMPIRE
                ? character("1000")
                : character("2001");

        public override IGraphQlResult<Interfaces.Human?> human(string id)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable<Interfaces.Review?>?> reviews(Interfaces.Episode episode)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<IEnumerable?> search(string? text)
        {
            throw new NotImplementedException();
        }

        public override IGraphQlResult<Interfaces.Starship?> starship(string id)
        {
            throw new NotImplementedException();
        }
    }

}
